using Fruit_PRJ.Extensions;
using Fruit_PRJ.Models;
using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_PRJ.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly ProductServices _productServices;

        public IndexModel(ProductServices productServices)
        {
            _productServices = productServices;
        }

        public List<Product> Products { get; set; } = new();

        public List<Category> Categories { get; set; } = new();
        public List<Origin> Origins { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Keyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? OriginId { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        public void OnGet()
        {
            Categories = _productServices.GetAllCategories();
            Origins = _productServices.GetAllOrigins();

            Products = _productServices.FilterShopProducts(
                Keyword,
                CategoryId,
                OriginId,
                MinPrice,
                MaxPrice,
                PageIndex,
                PageSize,
                out int total);

            TotalItems = total;
        }

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
