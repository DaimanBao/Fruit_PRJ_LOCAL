using Fruit_PRJ.Models;
using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_PRJ.Pages.Customer
{
    public class IndexModel : PageModel
    {
        private readonly AccountClientServices _accountService;

        public IndexModel(AccountClientServices accountService)
        {
            _accountService = accountService;
        }
            

        [BindProperty]
        public Account UserProfile { get; set; } = default!;

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("CustomerId");
            if (userId == null) return RedirectToPage("/Login_Logout/Index");

            // Gọi qua Service thay vì DbContext
            var user = _accountService.GetById(userId.Value);
            if (user == null) return NotFound();

            UserProfile = user;
            return Page();
        }

        public IActionResult OnPost()
        {
            var userId = HttpContext.Session.GetInt32("CustomerId");
            if (userId == null) return RedirectToPage("/Login_Logout/Index");

            var result = _accountService.UpdateProfile(userId.Value, UserProfile);

            if (result.Success)
            {
                // Cập nhật Session để hiển thị trên Layout ngay lập tức
                HttpContext.Session.SetString("CustomerName", UserProfile.Username);
                SuccessMessage = "Cập nhật hồ sơ thành công!";
                return RedirectToPage(); // Post-Redirect-Get pattern
            }

            ErrorMessage = result.Error;
            return Page();
        }
    }
}
