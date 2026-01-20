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
            if (id == null) RedirectToPage("/Products/Index");
            Product = _productServices.GetProductById(id.Value);

            if (Product == null || Product.IsDeleted) RedirectToPage("/Products/Index");
        }

        public IActionResult OnPostAddToCart(int id, int qty)
        {
            var product = _productServices.GetProductById(id);
            if (product != null)
            {
                CartHelper.Add(HttpContext.Session, product, qty);
            }
            return RedirectToPage("/Cart/Index");
        }

        public IActionResult OnPostBuyNow(int id, int qty)
        {
            var product = _productServices.GetProductById(id);
            if (product != null)
            {
                CartHelper.Add(HttpContext.Session, product, qty);
                return RedirectToPage("/Checkout/Index");
            }
            return RedirectToPage("/Products/Index");
        }
    }
}
