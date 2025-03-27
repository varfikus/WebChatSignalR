using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebChatSignalR.Data;
using WebChatSignalR.Models;

namespace WebChatSignalR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ChatDbContext _context;

        public HomeController(ILogger<HomeController> logger, ChatDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var user = new List<AppUser>();
            if (User.Identity.IsAuthenticated)
            {
                user = _context.Users.ToList()
                        .Cast<AppUser>()
                        .Where(x => x.Id != Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                        .ToList();

            }
            return View(user);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
