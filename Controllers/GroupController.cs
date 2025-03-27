using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebChatSignalR.Data;
using WebChatSignalR.Hubs;
using WebChatSignalR.Models;
using WebChatSignalR.Utils.Pagination;
using WebChatSignalR.ViewModels;

namespace WebChatSignalR.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        private readonly ChatDbContext _dbContext;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IWebHostEnvironment _env;

        public GroupController(ChatDbContext dbContext, IHubContext<ChatHub> hubContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
            _env = env;
        }

        [Route("[Controller]/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            const int page = 1;
            const int pageSize = 20;
            var loginUserId = CurrentLoginUser();

            if (id == loginUserId)
            {
                return RedirectToActionPermanent(nameof(Index));
            }

            // Fetch connected groups
            var connectedGroups = await _dbContext.Groups
                .Include(x => x.Creator)
                .Include(x => x.Members).ThenInclude(gm => gm.User)
                .Where(x => x.CreatorId == loginUserId || x.Members.Any(gm => gm.UserId == loginUserId))
                .OrderByDescending(x => x.Id)
                .Select(x => new GroupViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreatorId = x.CreatorId,
                    AllUsers = x.Members.Select(gm => new PersonViewModel
                    {
                        Id = gm.UserId,
                        Name = gm.User.Name,
                        Avatar = gm.User.Avatar
                    }).ToList()
                })
                .GetPagedAsync(page, pageSize);

            // Prepare conversation data
            var groupConversation = new GroupConversationViewModel();

            if (id.HasValue)
            {
                var currentGroup = await _dbContext.Groups
                    .Include(x => x.Creator)
                    .Include(x => x.Members).ThenInclude(gm => gm.User)
                    .Where(x => x.Id == id)
                    .Select(x => new GroupViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        CreatorId = x.CreatorId,
                        AllUsers = x.Members.Select(gm => new PersonViewModel
                        {
                            Id = gm.UserId,
                            Name = gm.User.Name,
                            Avatar = gm.User.Avatar
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (currentGroup == null)
                {
                    var newGroup = new Group
                    {
                        CreatorId = loginUserId,
                        Name = "New Group",
                        IsBlocked = false,
                        IsReported = false
                    };

                    await _dbContext.Groups.AddAsync(newGroup);
                    await _dbContext.SaveChangesAsync();

                    currentGroup = new GroupViewModel
                    {
                        Id = newGroup.Id,
                        Name = newGroup.Name,
                        CreatorId = newGroup.CreatorId
                    };

                    connectedGroups.Results.Insert(0, currentGroup);
                }
                else
                {
                    groupConversation = new GroupConversationViewModel
                    {
                        Id = currentGroup.Id,
                        Name = currentGroup.Name,
                        CreatorId = currentGroup.CreatorId,

                        // Ensure Members contain full user info
                        Members = currentGroup.AllUsers.Select(u => new GroupMemberViewModel
                        {
                            UserId = (int)u.Id,
                            User = new PersonViewModel
                            {
                                Id = u.Id,
                                Name = u.Name,
                                Avatar = u.Avatar
                            }
                        }).ToList(),

                        // Fetch Messages
                        Messages = await _dbContext.Messages
        .Where(x => x.GroupId == currentGroup.Id)
        .OrderByDescending(x => x.Timestamp)
        .Select(x => new MessageViewModel
        {
            Id = x.Id,
            Content = x.Content,
            SenderId = x.UserId,
            Timestamp = x.Timestamp,
            File = x.File,
            FileName = x.FileName
        })
        .GetPagedAsync(page, pageSize),

                        VoiceMessages = await _dbContext.VoiceMessages
        .Where(x => x.GroupId == currentGroup.Id) 
        .OrderByDescending(x => x.Timestamp)
        .Select(x => new VoiceMessageViewModel
        {
            Id = x.Id,
            SenderId = x.UserId,
            Timestamp = x.Timestamp,
            FileName = x.FileName,
            FilePath = x.FilePath
        })
        .GetPagedAsync(page, pageSize)
                    };
                }
            }

            return View(new GroupChatViewModel
            {
                Groups = connectedGroups,
                GroupConversation = groupConversation,
                CurrentUser = new PersonViewModel { Id = loginUserId }
            });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            if (message == null || (string.IsNullOrWhiteSpace(message.Content) && message.File?.Length == 0))
            {
                return BadRequest("Message content or file is required.");
            }

            var sendMessage = new Message
            {
                Content = string.IsNullOrWhiteSpace(message.Content) ? null : message.Content,
                RoomId = null,
                Timestamp = DateTime.UtcNow,
                UserId = message.UserId,
                File = message.File?.Length > 0 ? message.File : null,
                FileName = string.IsNullOrWhiteSpace(message.FileName) ? null : message.FileName,
                GroupId = message.GroupId
            };

            await _dbContext.Messages.AddAsync(sendMessage);
            await _dbContext.SaveChangesAsync();
            await _hubContext.Clients.Groups(sendMessage.GroupId.ToString())
                .SendAsync("ReceiveMessage", sendMessage.UserId, sendMessage.Content,
                    sendMessage.File?.Length > 0 ? sendMessage.File : null,
                    sendMessage.FileName, sendMessage.Timestamp);

            return Ok(sendMessage);
        }

        [HttpPost]
        public async Task<IActionResult> UploadVoiceMessage([FromForm] IFormFile audio, [FromForm] int GroupId, [FromForm] int UserId)
        {
            if (audio == null || audio.Length == 0)
            {
                return BadRequest("Файл отсутствует");
            }

            var FileName = $"{Guid.NewGuid()}.webm";
            var directoryPath = Path.Combine("wwwroot", "uploads", "voice");
            Directory.CreateDirectory(directoryPath);
            var FilePath = Path.Combine(directoryPath, FileName);

            using (var fileStream = new FileStream(FilePath, FileMode.Create))
            {
                await audio.CopyToAsync(fileStream);
            }

            var voiceMessage = new VoiceMessage
            {
                Timestamp = DateTime.UtcNow,
                UserId = UserId,
                RoomId = null,
                GroupId = GroupId,
                FileName = FileName,
                FilePath = $"/uploads/voice/{FileName}"
            };

            _dbContext.VoiceMessages.Add(voiceMessage);
            await _dbContext.SaveChangesAsync();

            await _hubContext.Clients.Group(GroupId.ToString())
                .SendAsync("ReceiveVoiceMessage", UserId, voiceMessage.FilePath, voiceMessage.Timestamp);

            return Ok(new { voiceMessage.FilePath });
        }

        [HttpGet("GetGroupUsers/{GroupId}")]
        public async Task<IActionResult> GetGroupUsers(int GroupId)
        {
            var groupExists = await _dbContext.Groups.AnyAsync(g => g.Id == GroupId);
            if (!groupExists)
            {
                return NotFound($"Group with ID {GroupId} not found.");
            }

            var users = await _dbContext.GroupMembers
                .Where(gm => gm.GroupId == GroupId)
                .Select(gm => new
                {
                    Id = gm.User.Id,
                    Name = gm.User.Name,
                    Avatar = gm.User.Avatar ?? "/images/default-avatar.png"
                })
                .ToListAsync();

            if (!users.Any())
            {
                return NotFound($"No users found for group {GroupId}");
            }

            return Ok(users);
        }

        [HttpGet]
        public async Task<IActionResult> CreateGroup()
        {
            var currentUser = CurrentLoginUser();
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var model = new GroupViewModel
            {
                CreatorId = currentUser,
                AllUsers = await _dbContext.Users
                            .OfType<AppUser>()
                            .Select(u => new PersonViewModel { Id = u.Id, Name = u.Name }) 
                            .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(GroupViewModel model)
        {
            var currentUser = CurrentLoginUser();
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var group = new Group
                {
                    Name = model.Name,
                    CreatorId = currentUser,
                    IsBlocked = false,
                    IsReported = false
                };

                _dbContext.Groups.Add(group);
                await _dbContext.SaveChangesAsync();

                foreach (var UserId in model.SelectedUserIds)
                {
                    var groupMember = new GroupMember
                    {
                        GroupId = group.Id,
                        UserId = UserId
                    };
                    _dbContext.GroupMembers.Add(groupMember);
                }

                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            model.AllUsers = await _dbContext.Users
                .Select(u => new PersonViewModel { Id = u.Id, Name = u.UserName })
                .ToListAsync();

            return View(model);
        }


        [HttpGet("[Controller]/[Action]/{id}")]
        public async Task<IActionResult> LoadHistory(int id, int page = 1)
        {
            var pageSize = 20;

            var messages = await _dbContext.Messages
                .Where(x => x.RoomId == id)  
                .Select(x => new MessageViewModel
                {
                    Id = x.Id,
                    Content = x.Content,
                    Timestamp = x.Timestamp,
                    SenderId = x.UserId,
                    File = x.File,
                    FileName = x.FileName
                })
                .OrderByDescending(x => x.Timestamp)
                .GetPagedAsync(page, pageSize);

            return Ok(messages);
        }

        //[HttpGet("[Controller]/[Action]/{id}")]
        //public async Task<IActionResult> GetGroupInfo(int id)
        //{
        //    // Get group information, including the creator and members
        //    var group = await _dbContext.Groups
        //        .Include(x => x.Members)  // Assuming a 'Members' collection for users in the group
        //        .Include(x => x.Creator)  // Creator of the group
        //        .Where(r => r.Id == id)
        //        .Select(x => new GroupViewModel
        //        {
        //            Id = x.Id,
        //            Name = x.Name,
        //            Creator = new PersonViewModel
        //            {
        //                Id = x.Creator.Id,
        //                Name = x.Creator.Name,
        //                Avatar = x.Creator.Avatar
        //            },
        //            Members = x.Members.Select(m => new PersonViewModel
        //            {
        //                Id = m.Id
        //            }).ToList()
        //        })
        //        .FirstOrDefaultAsync();

        //    if (group == null)
        //    {
        //        return NotFound("Group not found");
        //    }

        //    return Ok(group);
        //}

        private int CurrentLoginUser()
        {
            return Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
