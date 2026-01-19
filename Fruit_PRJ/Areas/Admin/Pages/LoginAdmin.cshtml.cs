using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_PRJ.Areas.Admin.Pages
{
    public class LoginAdminModel : PageModel
    {
        private readonly AccountServices _accountServices;

        public LoginAdminModel(AccountServices accountServices)
        {
            _accountServices = accountServices;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                Message = "Vui lòng nhập đầy đủ thông tin.";
                return Page();
            }

            var result = _accountServices.LoginAdmin(Email, Password);

            if (!result.Success)
            {
                Message = result.Error;
                return Page();
            }

            // Save session
            HttpContext.Session.SetInt32("AdminId", result.Account.Id);
            HttpContext.Session.SetString("AdminName", result.Account.Username);
            HttpContext.Session.SetInt32("AdminRole", result.Account.Role);

            return RedirectToPage("/Index");
        }
    }
}
