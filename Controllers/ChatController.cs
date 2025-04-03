using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebChatSignalR.Data;
using WebChatSignalR.Hubs;
using WebChatSignalR.Models;
using WebChatSignalR.Utils.Pagination;
using WebChatSignalR.ViewModels;
using System.Security.AccessControl;
using System.IO;

namespace WebChatSignalR.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ChatDbContext _dbContext;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IWebHostEnvironment _env;

        public ChatController(ChatDbContext dbContext, IHubContext<ChatHub> hubContext, IWebHostEnvironment env)
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

            if (id != null && id == loginUserId)
            {
                id = null;
                return RedirectToActionPermanent(nameof(Index), new { id });
            }

            var connectedRooms = await _dbContext.Rooms
                .Include(x => x.User)
                .Include(x => x.Creator)
                .Where(x => x.CreatorId == loginUserId || x.UserId == loginUserId)
                .OrderByDescending(x => x.UpdatedDate)
                .Select(x => new RoomViewModel
                {
                    Id = x.Id,
                    Sender = new PersonViewModel
                    {
                        Id = x.UserId != null && x.UserId != loginUserId ? x.UserId : x.CreatorId,
                        Name = x.UserId != null && x.UserId != loginUserId ? x.User.Name : x.Creator.Name,
                        Avatar = x.UserId != null && x.UserId != loginUserId ? x.User.Avatar : x.Creator.Avatar
                    },
                    Recipient = new PersonViewModel
                    {
                        Id = x.UserId != null && x.UserId == loginUserId ? x.UserId : x.CreatorId,
                        Name = x.UserId != null && x.UserId == loginUserId ? x.User.Name : x.Creator.Name,
                        Avatar = x.UserId != null && x.UserId == loginUserId ? x.User.Avatar : x.Creator.Avatar
                    },
                    UpdatedDate = x.UpdatedDate,
                    UpdateBy = x.UpdatedBy,
                    UnreadCount = x.UnreadCount,
                    IsBlocked = x.IsBlocked,
                    IsReported = x.IsReported,
                    BlockedBy = x.BlockedBy,
                    Excerpt = x.Messages.OrderByDescending(m => m.Timestamp).FirstOrDefault().Content
                })
                .OrderByDescending(x => x.UpdatedDate)
                .GetPagedAsync(page, pageSize);

            var conversation = new ConversationViewModel();

            if (id != null && await _dbContext.Users.AnyAsync(x => x.Id == id))
            {
                var currentRoom = await _dbContext.Rooms
                    .Include(x => x.User)
                    .Include(x => x.Creator)
                    .Where(x => x.CreatorId == loginUserId || x.UserId == loginUserId)
                    .Select(x => new RoomViewModel
                    {
                        Id = x.Id,
                        Sender = new PersonViewModel
                        {
                            Id = x.UserId != null && x.UserId != loginUserId ? x.UserId : x.CreatorId,
                            Name = x.UserId != null && x.UserId != loginUserId ? x.User.Name : x.Creator.Name,
                            Avatar = x.UserId != null && x.UserId != loginUserId ? x.User.Avatar : x.Creator.Avatar
                        },
                        Recipient = new PersonViewModel
                        {
                            Id = x.UserId != null && x.UserId == loginUserId ? x.UserId : x.CreatorId,
                            Name = x.UserId != null && x.UserId == loginUserId ? x.User.Name : x.Creator.Name,
                            Avatar = x.UserId != null && x.UserId == loginUserId ? x.User.Avatar : x.Creator.Avatar
                        },
                        UpdatedDate = x.UpdatedDate,
                        UpdateBy = x.UpdatedBy,
                        UnreadCount = x.UnreadCount,
                        BlockedBy = x.BlockedBy,
                        IsBlocked = x.IsBlocked,
                        IsReported = x.IsReported,
                        Excerpt = x.Messages.OrderByDescending(m => m.Timestamp).FirstOrDefault().Content
                    })
                    .FirstOrDefaultAsync(x => x.Sender.Id == id);

                if (currentRoom == null)
                {
                    var newRoom = new Room { CreatorId = loginUserId, UserId = (int)id, UpdatedDate = DateTime.UtcNow };
                    await _dbContext.Rooms.AddAsync(newRoom);
                    await _dbContext.SaveChangesAsync();

                    currentRoom = await _dbContext.Rooms
                        .Include(x => x.User)
                        .Include(x => x.Creator)
                        .Select(x => new RoomViewModel
                        {
                            Id = x.Id,
                            Sender = new PersonViewModel
                            {
                                Id = x.UserId != null && x.UserId != loginUserId ? x.UserId : x.CreatorId,
                                Name = x.UserId != null && x.UserId != loginUserId ? x.User.Name : x.Creator.Name,
                                Avatar = x.UserId != null && x.UserId != loginUserId ? x.User.Avatar : x.Creator.Avatar
                            },
                            Recipient = new PersonViewModel
                            {
                                Id = x.UserId != null && x.UserId == loginUserId ? x.UserId : x.CreatorId,
                                Name = x.UserId != null && x.UserId == loginUserId ? x.User.Name : x.Creator.Name,
                                Avatar = x.UserId != null && x.UserId == loginUserId ? x.User.Avatar : x.Creator.Avatar
                            },
                            UpdatedDate = x.UpdatedDate,
                            UpdateBy = x.UpdatedBy,
                        })
                        .FirstOrDefaultAsync(x => x.Id == newRoom.Id);

                    conversation.Id = currentRoom.Id.ToString();
                    conversation.Sender = currentRoom.Sender;
                    conversation.Recipient = currentRoom.Recipient;
                    connectedRooms.Results.Insert(0, currentRoom);
                }
                else if (currentRoom != null)
                {
                    conversation.Id = currentRoom.Id.ToString();
                    conversation.IsBlocked = currentRoom.IsBlocked;
                    conversation.IsReported = currentRoom.IsReported;
                    conversation.BlockedBy = currentRoom.BlockedBy;
                    conversation.Recipient = currentRoom.Recipient;
                    conversation.Sender = currentRoom.Sender;
                    //conversation.Sender = currentRoom.Sender ?? new PersonViewModel();
                    //conversation.Recipient = currentRoom.Recipient ?? new PersonViewModel();

                    var textMessages = await _dbContext.Messages
                        .Where(x => x.RoomId == currentRoom.Id)
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
                        .GetPagedAsync(page, pageSize);

                    var voiceMessages = await _dbContext.VoiceMessages
                        .Where(x => x.RoomId == currentRoom.Id)
                        .OrderByDescending(x => x.Timestamp)
                        .Select(x => new VoiceMessageViewModel
                        {
                            Id = x.Id,
                            FilePath = x.FilePath,
                            Timestamp = x.Timestamp,
                            SenderId = x.UserId,
                            FileName = x.FileName
                        })
                        .GetPagedAsync(page, pageSize);

                    conversation.Messages = textMessages;
                    conversation.VoiceMessages = voiceMessages;
                }
            }

            var chat = new ChatViewModel { Rooms = connectedRooms, Conversation = conversation };

            return View(chat);
        }

        [HttpPut]
        public async Task<bool> ReadMessage([FromBody] int conversationId)
        {
            var loginUserId = CurrentLoginUser();
            var room = await _dbContext.Rooms.FirstOrDefaultAsync(x => x.Id == conversationId &&
                                                                       (x.UserId == loginUserId ||
                                                                        x.CreatorId == loginUserId));
            if (room == null) return false;
            room.UnreadCount = 0;
            _dbContext.Rooms.Update(room);
            await _dbContext.SaveChangesAsync();
            return true;

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
                RoomId = message.RoomId,
                Timestamp = DateTime.UtcNow,
                UserId = message.UserId,
                File = message.File?.Length > 0 ? message.File : null,
                FileName = string.IsNullOrWhiteSpace(message.FileName) ? null : message.FileName,
                GroupId = null
            };

            await _dbContext.Messages.AddAsync(sendMessage);
            await _dbContext.SaveChangesAsync();
            await _hubContext.Clients.Groups(sendMessage.RoomId.ToString())
                .SendAsync("ReceiveMessage", sendMessage.UserId, sendMessage.Content,
                    sendMessage.File?.Length > 0 ? sendMessage.File : null,
                    sendMessage.FileName, sendMessage.Timestamp);

            return Ok(sendMessage);
        }

        [HttpPost]
        [Route("Chat/UploadVoiceMessage")]
        public async Task<IActionResult> UploadVoiceMessage([FromForm] IFormFile audio, [FromForm] int RoomId, [FromForm] int UserId)
        {
            if (audio == null || audio.Length == 0)
            {
                return BadRequest("Файл отсутствует");
            }

            var FileName = $"{Guid.NewGuid()}.webm";
            var directoryPath = Path.Combine("wwwroot", "uploads", "voice");
            var FilePath = Path.Combine(directoryPath, FileName);

            using (var fileStream = new FileStream(FilePath, FileMode.Create))
            {
                await audio.CopyToAsync(fileStream);
            }

            var voiceMessage = new VoiceMessage
            {
                Timestamp = DateTime.UtcNow,
                UserId = UserId,
                RoomId = RoomId,
                GroupId = null,
                FileName = FileName,
                FilePath = $"/uploads/voice/{FileName}"
            };

            _dbContext.VoiceMessages.Add(voiceMessage);
            await _dbContext.SaveChangesAsync();

            await _hubContext.Clients.Group(RoomId.ToString())
                .SendAsync("ReceiveVoiceMessage", UserId, voiceMessage.FilePath, voiceMessage.Timestamp);

            return Ok(new { voiceMessage.FilePath });
        }

        [HttpPost("[Controller]/[Action]/{RoomId}")]
        public async Task<ActionResult> BlockUser(int RoomId, bool IsReported = false)
        {
            var room = await _dbContext.Rooms.FirstOrDefaultAsync(x => x.Id == RoomId);
            if (room == null)
                return BadRequest(new { Success = false, Message = "You are not able to use this operation" });
            room.IsBlocked = !room.IsBlocked;
            room.IsReported = IsReported && room.IsBlocked;
            room.BlockedBy = room.IsBlocked ? CurrentLoginUser() : 0;
            room.UpdatedBy = CurrentLoginUser();
            _dbContext.Rooms.Update(room);
            await _dbContext.SaveChangesAsync();
            return Ok(new
            {
                Success = true,
                Message = "You block this conversation now this user cannot message you anymore"
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteChat(int RoomId)
        {
            var room = await _dbContext.Rooms.Where(x => x.Id == RoomId).FirstOrDefaultAsync();
            _dbContext.Rooms.Remove(room);
            await _dbContext.SaveChangesAsync();
            return Ok(room.Id);
        }

        [HttpGet("[Controller]/[Action]/{id}")]
        public async Task<IActionResult> LoadHistory(int id, int page = 1)
        {
            var pageSize = 20;
            var messages = await _dbContext.Messages.Where(x => x.RoomId == id)
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
        [HttpGet("[Controller]/[Action]/{id}")]
        public async Task<IActionResult> GetChatUser(int id)
        {
            var room =await _dbContext.Rooms.Include(x=>x.User).Include(x=>x.Creator).Select(x=> new RoomViewModel
            {
                Id = x.Id,
                Sender = new PersonViewModel
                {
                    Id = x.User.Id!=CurrentLoginUser()? x.User.Id : x.Creator.Id,
                    Name = x.User.Id!=CurrentLoginUser()? x.User.Name : x.Creator.Name,
                    Avatar = x.User.Id!=CurrentLoginUser()? x.User.Avatar : x.Creator.Avatar,
                }
    
            }).FirstOrDefaultAsync(r => r.Id == id);
            return Ok(room.Sender);
        }

        private int CurrentLoginUser()
        {
            return Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}