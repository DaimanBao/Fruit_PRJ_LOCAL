using Fruit_PRJ.Extensions;
using Fruit_PRJ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_PRJ.Pages.Cart
{
    public class IndexModel : PageModel
    {
        public List<CartItem> CartItems { get; set; } = new();
        public decimal GrandTotal => CartItems.Sum(x => x.Total);

        [TempData]
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            LoadCart();
        }

        private void LoadCart()
        {
            CartItems = HttpContext.Session.GetObject<List<CartItem>>("CART") ?? new();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetObject("CART", cart);
            HttpContext.Session.SetInt32("CartCount", cart.Sum(x => x.Quantity));
        }

        public IActionResult OnPostIncrease(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("CART") ?? new();
            var item = cart.FirstOrDefault(x => x.ProductId == id); // Dùng FirstOrDefault để an toàn

            if (item != null)
            {
                item.Quantity++;
                SaveCart(cart);
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDecrease(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("CART") ?? new();
            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item != null)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                    cart.Remove(item);

                SaveCart(cart);
            }
            return RedirectToPage();
        }

        public IActionResult OnPostRemove(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("CART") ?? new();
            cart.RemoveAll(x => x.ProductId == id);

            SaveCart(cart);
            return RedirectToPage();
        }

        // Kiểm tra đăng nhập trước khi đi thanh toán
        public IActionResult OnGetCheckout()
        {
            // 1. Kiểm tra login thủ công
            var userId = HttpContext.Session.GetInt32("CustomerId");
            if (userId == null)
            {
                ErrorMessage = "Vui lòng đăng nhập để tiến hành thanh toán!";
                return RedirectToPage("/Login_Logout/Login", new { returnUrl = "/gio-hang" });
            }

            // 2. Kiểm tra giỏ hàng có đồ không
            var cart = HttpContext.Session.GetObject<List<CartItem>>("CART") ?? new();
            if (!cart.Any())
            {
                ErrorMessage = "Giỏ hàng của bạn đang trống!";
                return RedirectToPage();
            }

            return RedirectToPage("/Checkout/Index");
        }
    }
}
