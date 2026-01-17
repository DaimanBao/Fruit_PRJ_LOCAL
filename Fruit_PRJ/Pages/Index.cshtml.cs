using Fruit_PRJ.Models;
using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_PRJ.Pages
{
    
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ProductServices _productServices;


        public IndexModel(ILogger<IndexModel> logger, ProductServices productServices)
        {
            _logger = logger;
            _productServices = productServices;
        }

        public List<Product> products { get; set; } = new List<Product>();

        public void OnGet()
        {
            products = _productServices.GetAllProducts();

        }
    }
}
