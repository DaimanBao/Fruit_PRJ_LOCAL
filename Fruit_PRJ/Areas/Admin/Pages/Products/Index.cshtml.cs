using Fruit_PRJ.Models;
using Fruit_PRJ.Services;
using Fruit_Store_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_Store_PRJ.Areas.Admin.Pages.Products
{
    public class IndexModel : PageModel
    {
        //Services
        private readonly ProductServices _productServices;
        private readonly ImageServices _imageServices;
        private readonly UtilitiesServices _utilitiesServices;


        //Messages
        public string Message { get; set; }

        //Statistics
        public int TotalProducts { get; set; }
        public int SellingProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public int StopSellingProducts { get; set; }


        //Filters
        [BindProperty(SupportsGet = true)]
        public string? SearchKeyword { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? FilterCategoryId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? FilterOriginId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? FilterUnitId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? FilterStatus { get; set; }

        //Pagination
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalItems { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalItems / PageSize);






        public IndexModel(ProductServices productServices, ImageServices imageServices, UtilitiesServices utilitiesServices)
        {
            _productServices = productServices;
            _imageServices = imageServices;
            _utilitiesServices = utilitiesServices;
        }

        private void LoadBaseData()
        {
            categories = _productServices.GetAllCategories();
            origins = _productServices.GetAllOrigins();
            units = _productServices.GetAllUnits();
            products = _productServices.GetAllProducts();
        }


        public void OnGet()
        {
            CheckLogin();

            LoadBaseData();

            products = _productServices.FilterProductsPaging(
              SearchKeyword,
              FilterCategoryId,
              FilterOriginId,
              FilterUnitId,
              FilterStatus,
              PageIndex,
              PageSize,
              out int total);

            TotalItems = total;

            var stat = _productServices.GetProductStatistics();
            TotalProducts = stat.Total;
            SellingProducts = stat.Selling;
            OutOfStockProducts = stat.OutOfStock;
            StopSellingProducts = stat.StopSelling;
        }



        //Category
        public List<Category> categories { get; set; } = new List<Category>();
        [BindProperty]
        public Category newCategory { get; set; } = new Category();

        public IActionResult OnPostAddCategory()
        {
            if(string.IsNullOrWhiteSpace(newCategory.Name))
            {
                Message = "Tên danh mục không được để trống.";
                LoadBaseData();
                return Page();
            }

            var result = _productServices.AddCategory(newCategory);

            if(!result.Success)
            {
                Message = result.Error!;
                LoadBaseData();
                return Page();
            }

            return RedirectToPage();
        }

        //Origin
        public List<Origin> origins { get; set; } = new List<Origin>();
        [BindProperty]
        public Origin newOrigin { get; set; } = new Origin();

        public IActionResult OnPostAddOrigin()
        {
            if (string.IsNullOrWhiteSpace(newOrigin.Name))
            {
                Message = "Tên xuất xứ không được để trống.";
                LoadBaseData();
                return Page();
            }
            var result = _productServices.AddOrigin(newOrigin);

            if (!result.Success)
            {
                Message = result.Error!;
                LoadBaseData();
                return Page();
            }

            return RedirectToPage();
        }

        //UNIT
        public List<Unit> units { get; set; } = new List<Unit>();
      
        [BindProperty]
        public Unit newUnit { get; set; } = new Unit();
        public IActionResult OnPostAddUnit()
        {
            if (string.IsNullOrWhiteSpace(newUnit.Name))
            {
                Message = "Tên đơn vị không được để trống.";
                LoadBaseData();
                return Page();
            }
            var result = _productServices.AddUnit(newUnit);
            if (!result.Success)
            {
                Message = result.Error!;
                LoadBaseData();
                return Page();
            }
            return RedirectToPage();
        }

        //PRODUCTS
        public List<Product> products { get; set; } = new List<Product>();
        [BindProperty]
        public Product? newProduct { get; set; } = new Product();
        [BindProperty]
        public List<IFormFile> UploadImages { get; set; } = new();


        public IActionResult OnPostAddProduct()
        {
            if (newProduct == null ||
                string.IsNullOrWhiteSpace(newProduct.Name) ||
                string.IsNullOrWhiteSpace(newProduct.Sku) ||
                string.IsNullOrWhiteSpace(newProduct.Description) ||
                newProduct.Price <= 0 ||
                newProduct.Stock < 0 ||
                newProduct.CategoryId <= 0 ||
                newProduct.OriginId <= 0 ||
                newProduct.UnitId <= 0
                )
            {
                Message = "Dữ liệu sản phẩm không hợp lệ.";
                LoadBaseData();
                return Page();
            }

            if (UploadImages == null || !UploadImages.Any())
            {
                Message = "Vui lòng chọn ít nhất 1 ảnh sản phẩm.";
                LoadBaseData();
                return Page();
            }


            List<ProductImage> images;

            try
            {
                images = _imageServices.SaveProductImages(UploadImages);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                LoadBaseData();
                return Page();
            }


            var result = _productServices.AddProduct(newProduct,images);
            if (!result.Success)
            {
                Message = result.Error!;
                LoadBaseData();
                return Page();
            }
            return RedirectToPage();
        }

        public string GetStatusText(int status)
        {
            return _utilitiesServices.GetProductStatusText(status);
        }
       
        public string GetStatusClass(int status)
        {
            return _utilitiesServices.GetStatusClass(status);
        }

        public IActionResult OnPostDeleteProduct(int id)
        {
            var result = _productServices.DeleteProduct(id);

            if (!result.Success)
            {
                Message = result.Error;
                LoadBaseData();
                return Page();
            }

            return RedirectToPage();
        }


        public void CheckLogin()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                Response.Redirect("/Admin/LoginAdmin");
        }

    }
}
