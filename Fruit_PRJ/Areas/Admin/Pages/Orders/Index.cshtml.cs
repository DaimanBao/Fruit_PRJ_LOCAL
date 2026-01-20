using Fruit_PRJ.Models;
using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_Store_PRJ.Areas.Admin.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly OrderServices _orderServices;

        public IndexModel(OrderServices orderServices)
        {
            _orderServices = orderServices;
        }

        public List<Order> Orders { get; set; } = new();
        public Dictionary<string, int> Stats { get; set; } = new();

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                return RedirectToPage("/LoginAdmin");

            Orders = _orderServices.GetAllOrdersAdmin();
            Stats = _orderServices.GetOrderStatistics();

            return Page();
        }

        public IActionResult OnPostUpdateStatus(int id, int status)
        {
            _orderServices.AdminUpdateStatus(id, status);
            return RedirectToPage();
        }

    }
}
