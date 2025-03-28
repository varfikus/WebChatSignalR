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
    public class CreateGroupController : Controller
    {
        private readonly ChatDbContext _dbContext;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IWebHostEnvironment _env;

        public CreateGroupController(ChatDbContext dbContext, IHubContext<ChatHub> hubContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
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

            return RedirectToAction("Index", "Group");
        }

        private int CurrentLoginUser()
        {
            return Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
