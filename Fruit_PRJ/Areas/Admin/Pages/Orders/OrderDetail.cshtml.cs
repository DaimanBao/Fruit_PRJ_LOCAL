using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fruit_PRJ.Models;

namespace Fruit_Store_PRJ.Areas.Admin.Pages.Orders
{
    public class OrderDetailModel : PageModel
    {
        private readonly OrderServices _orderServices;
        public OrderDetailModel(OrderServices orderServices) => _orderServices = orderServices;

        public Order Order { get; set; } = default!;

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                return RedirectToPage("/Admin/LoginAdmin");

            var order = _orderServices.GetOrderByIdAdmin(id);
            if (order == null) return NotFound();

            Order = order;
            return Page();
        }

        public IActionResult OnPostUpdateStatus(int id, int status)
        {
            _orderServices.AdminUpdateStatus(id, status);
            return RedirectToPage(new { id = id });
        }

    }
}
