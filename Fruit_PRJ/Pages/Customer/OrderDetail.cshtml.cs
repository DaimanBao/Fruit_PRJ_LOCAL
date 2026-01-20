using Fruit_PRJ.Models;
using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_PRJ.Pages.Customer
{
    public class OrderDetailModel : PageModel
    {
        private readonly OrderServices _orderServices;

        public OrderDetailModel(OrderServices orderServices)
        {
            _orderServices = orderServices;
        }

        public Order Order { get; set; } = default!;

        public IActionResult OnGet(string code)
        {
            if (string.IsNullOrEmpty(code)) return RedirectToPage("/Customer/Orders");

            var userId = HttpContext.Session.GetInt32("CustomerId");
            if (userId == null) return RedirectToPage("/Login_Logout/Index");

            var order = _orderServices.GetOrderByCode(code, userId.Value);

            if (order == null) return NotFound();

            Order = order;
            return Page();
        }

        public string GetPaymentMethodName(int method) => method switch
        {
            1 => "Thanh toán khi nhận hàng (COD)",
            2 => "Chuyển khoản ngân hàng",
            3 => "Thanh toán Online (Stripe)",
            _ => "Không xác định"
        };
    }
}
