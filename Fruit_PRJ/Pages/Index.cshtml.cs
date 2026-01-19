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
            int pageSize = 10;

            var all = _productServices.GetAllProducts();

            TotalPages = (int)Math.Ceiling(all.Count / (double)pageSize);

            Products = all
                .Skip((PageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        // ---------------- CART ----------------

        public IActionResult OnPostAddToCart(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("CART") ?? new();

            var product = _productServices.GetProductById(id);
            if (product == null) return RedirectToPage();

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
            else
            {
                item.Quantity++;
            }

            HttpContext.Session.SetObject("CART", cart);

            return RedirectToPage();
        }

        public IActionResult OnPostBuyNow(int id)
        {
            OnPostAddToCart(id);
            return RedirectToPage("/Cart/Index");
        }


    }
}
