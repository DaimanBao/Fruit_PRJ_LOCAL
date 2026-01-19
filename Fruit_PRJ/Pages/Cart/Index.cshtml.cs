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

        public void OnGet()
        {
            LoadCart();
        }

        void LoadCart()
        {
            CartItems = HttpContext.Session.GetObject<List<CartItem>>("CART") ?? new();
        }

        public IActionResult OnPostIncrease(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("CART") ?? new();
            var item = cart.First(x => x.ProductId == id);
            item.Quantity++;

            HttpContext.Session.SetObject("CART", cart);
            return RedirectToPage();
        }

        public IActionResult OnPostDecrease(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("CART") ?? new();
            var item = cart.First(x => x.ProductId == id);

            item.Quantity--;
            if (item.Quantity <= 0)
                cart.Remove(item);

            HttpContext.Session.SetObject("CART", cart);
            return RedirectToPage();
        }

        public IActionResult OnPostRemove(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("CART") ?? new();
            cart.RemoveAll(x => x.ProductId == id);

            HttpContext.Session.SetObject("CART", cart);
            return RedirectToPage();
        }
    }
}
