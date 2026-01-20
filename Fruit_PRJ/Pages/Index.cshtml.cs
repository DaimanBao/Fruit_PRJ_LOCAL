using Fruit_PRJ.Models;
using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Fruit_PRJ.Extensions;

namespace Fruit_PRJ.Pages
{
    
    public class IndexModel : PageModel
    {
        private readonly ProductServices _productServices;

        public IndexModel(ProductServices productServices)
        {
            _productServices = productServices;
        }

        public List<Product> Products { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        public int TotalPages { get; set; }

        public void OnGet()
        {
            int pageSize = 12; // Tăng lên 12 để chia hết cho hàng 3 hoặc 4 sản phẩm
            int totalItems = 0;

            // SỬA: Chỉ lấy sản phẩm đang hoạt động (Status = 1) thay vì lấy tất cả
            Products = _productServices.FilterShopProducts(
                null, null, null, null, null,
                PageIndex, pageSize, out totalItems);

            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        }

        // ---------------- CART ----------------

        public IActionResult OnPostAddToCart(int id)
        {
            UpdateCartLogic(id);
            return RedirectToPage();
        }

        public IActionResult OnPostBuyNow(int id)
        {
            UpdateCartLogic(id);
            return RedirectToPage("/Cart/Index");
        }

        // Gom logic vào một hàm helper để dùng cho cả 2 nút
        private void UpdateCartLogic(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("CART") ?? new();
            var product = _productServices.GetProductById(id);
            if (product == null) return;

            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = 1,
                    Image = product.ProductImages.FirstOrDefault()?.ImageUrl
                });
            }
            else { item.Quantity++; }

            HttpContext.Session.SetInt32("CartCount", cart.Sum(x => x.Quantity));
            HttpContext.Session.SetObject("CART", cart);
        }

    }
}
