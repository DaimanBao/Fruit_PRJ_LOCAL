using Fruit_PRJ.Models;
using Fruit_PRJ.Services;
using Fruit_Store_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_Store_PRJ.Areas.Admin.Pages.Products
{
    public class ProductDetailModel : PageModel
    {
        private readonly ProductServices _productServices;
        private readonly UtilitiesServices _utilitiesServices;
        private readonly ImageServices _imageServices;

        public string Message { get; set; }

        public ProductDetailModel(ProductServices productServices, UtilitiesServices utilitiesServices, ImageServices imageServices)
        {
            _productServices = productServices;
            _utilitiesServices = utilitiesServices;
            _imageServices = imageServices;
        }
        public Product? product { get; set; }
        public string statusText { get; set; } = "";
        public string statusClass { get; set; } = "";
        public List<Category> categories { get; set; } = new List<Category>();
        public List<Origin> origins { get; set; } = new List<Origin>();
        public List<Unit> units { get; set; } = new List<Unit>();

        public void OnLoad()
        {
            categories = _productServices.GetAllCategories();
            origins = _productServices.GetAllOrigins();
            units = _productServices.GetAllUnits();
        }
        public IActionResult OnGet(int? id)
        {
            if (!id.HasValue)
                return RedirectToPage("Index");

            OnLoad();

            product = _productServices.GetProductById(id.Value);

            if (product == null)
                return RedirectToPage("Index");

            EditProduct = product;

            statusText = _utilitiesServices.GetProductStatusText(product.Status);
            statusClass = _utilitiesServices.GetStatusClass(product.Status);
            Message = TempData["Message"] as string;

            return Page();
        }


        //Edit Product-------------------------------------
        [BindProperty]
        public Product EditProduct { get; set; } = new Product();
        [BindProperty]
        public List<IFormFile> NewImages { get; set; } = new List<IFormFile>();

        //Edit Product
        public IActionResult OnPostEditProduct()
        {
            OnLoad();

            if (EditProduct == null || EditProduct.Id <= 0)
            {
                TempData["Message"] = "Sản phẩm không hợp lệ.";
                return RedirectToPage("Index");
            }

            if (string.IsNullOrWhiteSpace(EditProduct.Name) ||
                string.IsNullOrWhiteSpace(EditProduct.Sku) ||
                EditProduct.Price <= 0 ||
                EditProduct.Stock < 0)
            {
                TempData["Message"] = "Dữ liệu sản phẩm không hợp lệ.";
                return RedirectToPage("ProductDetail", new { id = EditProduct.Id });
            }

            List<ProductImage>? newImages = null;

            // Upload new image
            if (NewImages != null && NewImages.Any())
            {
                try
                {
                    newImages = _imageServices.SaveProductImages(NewImages);
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                    return RedirectToPage("ProductDetail", new { id = EditProduct.Id });
                }
            }

            var result = _productServices.UpdateProduct(EditProduct, newImages);

            if (!result.Success)
            {
                TempData["Message"] = result.Error!;
                return RedirectToPage("ProductDetail", new { id = EditProduct.Id });
            }

            TempData["Message"] = "Cập nhật sản phẩm thành công.";
            return RedirectToPage("ProductDetail", new { id = EditProduct.Id });
        }


        // Change Status
        public IActionResult OnPostChangeStatus(int id, int status)
        {
            var product = _productServices.GetProductById(id);

            if (product == null)
            {
                TempData["Message"] = "Không tìm thấy sản phẩm.";
                return RedirectToPage("Index");
            }

            product.Status = status;
            _productServices.UpdateStatus(product); // method riêng, KHÔNG đụng EditProduct

            TempData["Message"] = "Cập nhật trạng thái thành công.";
            return RedirectToPage("ProductDetail", new { id });
        }

        // Set Main Image
        public IActionResult OnPostSetMainImage(int productId, int imageId)
        {
            var product = _productServices.GetProductById(productId);

            if (product == null)
            {
                TempData["Message"] = "Không tìm thấy sản phẩm.";
                return RedirectToPage("Index");
            }

            var image = product.ProductImages.FirstOrDefault(i => i.Id == imageId);
            if (image == null)
            {
                TempData["Message"] = "Không tìm thấy ảnh.";
                return RedirectToPage("ProductDetail", new { id = productId });
            }

            // Reset ảnh chính cũ
            foreach (var img in product.ProductImages)
                img.IsMain = false;

            // Set ảnh mới
            image.IsMain = true;

            _productServices.UpdateMainImage(product);

            TempData["Message"] = "Đã cập nhật ảnh chính.";
            return RedirectToPage("ProductDetail", new { id = productId });
        }

    }
}
