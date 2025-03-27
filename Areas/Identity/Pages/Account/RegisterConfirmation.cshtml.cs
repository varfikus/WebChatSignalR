using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using WebChatSignalR.Models;

namespace WebChatSignalR.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        // private readonly IEmailSender _sender;

        public RegisterConfirmationModel(UserManager<AppUser> userManager /*, IEmailSender sender*/)
        {
            _userManager = userManager;
            // _sender = sender;
        }

        public string Email { get; set; }

        public bool DisplayConfirmAccountLink { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;

            // Оставляем возможность показать ссылку подтверждения (для тестирования)
            DisplayConfirmAccountLink = true;

            if (DisplayConfirmAccountLink)
            {
                var UserId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", UserId = UserId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                // Логика отправки email отключена
                // await _sender.SendEmailAsync(Email, "Confirm your email",
                // $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(EmailConfirmationUrl)}'>clicking here</a>.");
            }

            return Page();
        }
    }
}
