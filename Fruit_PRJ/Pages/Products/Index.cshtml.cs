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

        public IActionResult OnPostAddToCart(int id, int qty = 1) // Thêm giá trị mặc định = 1
        {
            var product = _productServices.GetProductById(id);
            if (product != null)
            {
                CartHelper.Add(HttpContext.Session, product, qty);
            }
            return RedirectToPage("/Cart/Index");
        }

        public IActionResult OnPostBuyNow(int id)
        {
            OnPostAddToCart(id, 1); // Đảm bảo truyền đủ id và qty
            return RedirectToPage("/Cart/Index");
        }
    }
}
