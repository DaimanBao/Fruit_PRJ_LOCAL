using Fruit_PRJ.Models;
using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Fruit_PRJ.Pages.Login_Logout
{
    public class IndexModel : PageModel
    {
        // Sử dụng Service dành cho Client
        private readonly AccountClientServices _accountClientService;

        public IndexModel(AccountClientServices accountClientService)
        {
            _accountClientService = accountClientService;
        }

        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string Password { get; set; } = string.Empty;
        [BindProperty] public Account NewAccount { get; set; } = new();
        [BindProperty] public string ConfirmPassword { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet(bool? success)
        {
            if (success == true) SuccessMessage = "Đăng ký thành công! Bạn có thể đăng nhập ngay.";
        }

        // Xử lý ĐĂNG NHẬP dành cho Khách hàng
        // File: Pages/Login_Logout/IndexModel.cshtml.cs

        public IActionResult OnPostLogin()
        {
            var result = _accountClientService.LoginCustomer(Email, Password);
            if (!result.Success || result.Account == null)
            {
                ErrorMessage = result.Error;
                return Page();
            }

            // Lưu đầy đủ để trang Checkout dùng luôn
            HttpContext.Session.SetInt32("CustomerId", result.Account.Id);
            HttpContext.Session.SetString("CustomerName", result.Account.Username);
            HttpContext.Session.SetString("CustomerEmail", result.Account.Email);
            HttpContext.Session.SetInt32("CustomerRole", result.Account.Role);
            HttpContext.Session.SetString("CustomerPhone", result.Account.Phone ?? "");
            HttpContext.Session.SetString("CustomerAddress", result.Account.Address ?? "");

            return RedirectToPage("/Index");
        }

        // Xử lý ĐĂNG KÝ dành cho Khách hàng
        public IActionResult OnPostRegister()
        {
            if (NewAccount.PasswordHash != ConfirmPassword)
            {
                ErrorMessage = "Mật khẩu xác nhận không khớp.";
                return Page();
            }

            // Gọi hàm RegisterCustomer (hàm này đã mặc định Role = 3 bên trong Service)
            var result = _accountClientService.RegisterCustomer(NewAccount);

            if (result.Success)
            {
                return RedirectToPage(new { success = true });
            }

            ErrorMessage = result.Error;
            return Page();
        }
    }
}
