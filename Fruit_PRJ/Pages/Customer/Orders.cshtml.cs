using Fruit_PRJ.Models;
using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_PRJ.Pages.Customer
{
    public class OrdersModel : PageModel
    {
        private readonly OrderServices _orderServices;

        public OrdersModel(OrderServices orderServices)
        {
            _orderServices = orderServices;
        }

        public List<Order> CustomerOrders { get; set; } = new();

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("CustomerId");
            if (userId == null)
                return RedirectToPage("/Login_Logout/Index", new { returnUrl = "/Customer/Orders" });

            CustomerOrders = _orderServices.GetOrdersByCustomer(userId.Value);

            return Page();
        }

        public string GetStatusClass(int status) => status switch
        {
            1 => "bg-warning text-dark", 
            2 => "bg-primary",           
            3 => "bg-success",           
            4 => "bg-danger",            
            _ => "bg-secondary"
        };

        public string GetStatusName(int status) => status switch
        {
            1 => "Chờ xử lý",
            2 => "Đang giao hàng",
            3 => "Hoàn thành",
            4 => "Đã hủy",
            _ => "Không xác định"
        };
    }
}
