using Fruit_PRJ.Extensions;
using Fruit_PRJ.Models;
using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_PRJ.Pages.Products
{
    public class DetailModel : PageModel
    {
        private readonly ProductServices _productServices;

        public DetailModel(ProductServices productServices)
        {
            _productServices = productServices;
        }

        public Product? Product { get; set; }

        public void OnGet(int? id)
        {
            Product = _productServices.GetProductById(id.Value);
        }

        public IActionResult OnPostAddToCart(int id, int qty)
        {
            CartHelper.Add(HttpContext.Session, id, qty);
            return RedirectToPage("/Cart/Index");
        }

        public IActionResult OnPostBuyNow(int id, int qty)
        {
            CartHelper.Add(HttpContext.Session, id, qty);
            return RedirectToPage("/Checkout/Index");
        }
    }
}
