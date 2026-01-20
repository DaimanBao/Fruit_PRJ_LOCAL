using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fruit_PRJ.Models;

namespace Fruit_Store_PRJ.Areas.Admin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly OrderServices _orderServices;
        private readonly ProductServices _productServices;
        private readonly AccountServices _accountServices;

        public IndexModel(OrderServices orderServices, ProductServices productServices, AccountServices accountServices)
        {
            _orderServices = orderServices;
            _productServices = productServices;
            _accountServices = accountServices;
        }

        // Cac thuoc tinh hien thi 
        public Dictionary<string, int> OrderStats { get; set; } = null!;
        public ProductServices.ProductStatisticDto ProductStats { get; set; } = null!;
        public AccountServices.AccountStatisticDto AccountStats { get; set; } = null!;
        public List<Order> RecentOrders { get; set; } = null!;
        public decimal TotalRevenue { get; set; }

        public IActionResult OnGet()
        {
            //Kiem tra Login
            if (HttpContext.Session.GetInt32("AdminId") == null)
                return RedirectToPage("/LoginAdmin", new { area = "Admin" });

            OrderStats = _orderServices.GetOrderStatistics();
            ProductStats = _productServices.GetProductStatistics();
            AccountStats = _accountServices.GetAccountStatistic();

            //Lay 5 don hang moi nhat
            RecentOrders = _orderServices.GetAllOrdersAdmin().Take(5).ToList();

            //Tinh tong doanh thu
            TotalRevenue = _orderServices.GetAllOrdersAdmin()
                            .Where(o => o.Status == 3)
                            .Sum(o => o.TotalAmount);

            return Page();
        }

    }
}
