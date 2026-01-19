using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_Store_PRJ.Areas.Admin.Pages.Orders
{
    public class OrderDetailModel : PageModel
    {
        public void OnGet()
        {
            CheckLogin();

        }

        public void CheckLogin()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                Response.Redirect("/Admin/LoginAdmin");
        }

    }
}
