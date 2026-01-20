using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_PRJ.Pages.Login_Logout
{
    public class LogoutModel : PageModel
    {
        // File: Pages/Login_Logout/Logout.cshtml.cs

        public IActionResult OnGet()
        {
            // Xóa sạch Session của Client
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }
    }
}
