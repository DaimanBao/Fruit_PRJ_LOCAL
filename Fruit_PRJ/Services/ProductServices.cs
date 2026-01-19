using Fruit_PRJ.Models;
using Fruit_Store_PRJ.Services;
using Microsoft.EntityFrameworkCore;

namespace Fruit_PRJ.Services
{
    public class ProductServices
    {
        private readonly FruitStoreDbContext _context;
        private readonly ImageServices _imageServices;
        private readonly UtilitiesServices _utilitiesServices;

        public class ServiceResult
        {
            public bool Success { get; set; }
            public string? Error { get; set; }
        }


        //Constructor
        public ProductServices(FruitStoreDbContext context, UtilitiesServices utilitiesServices, ImageServices imageServices)
        {
            _context = context;
            _utilitiesServices = utilitiesServices;
            _imageServices = imageServices;
        }

        //ADMIN///////////////////////////////////////////////////////
        //Category//--------------------------------------------- 

        //Get All Categories
        public List<Category> GetAllCategories()
        {
            return _context.Categories.
                Where(c => !c.IsDeleted).
                ToList();
        }

        //Get Category by Id

        public Category? GetCategoryById(int categoryId)
        {
            return _context.Categories.Find(categoryId);
        }

        // Add Category
        public ServiceResult AddCategory(Category category)
        {
            category.Name = _utilitiesServices.CleanDataInput(category.Name, false, false, true, true, true);


            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return new ServiceResult
                {
                    Success = false,
                    Error = "Tên danh mục không hợp lệ."
                };
            }

            bool isExist = _context.Categories.Any(c =>
                   c.Name.ToLower() == category.Name.ToLower() &&
                   !c.IsDeleted
               );

            if (isExist) return new ServiceResult
            {
                Success = false,
                Error = "Tên danh mục đã tồn tại"
            };

            category.IsActive = true;
            category.IsDeleted = false;
            category.CreatedAt = DateTime.Now;
            _context.Categories.Add(category);
            _context.SaveChanges();

            return new ServiceResult
            {
                Success = true
            };
        }

        //Origin//---------------------------------------------

        //Get All Origins
        public List<Origin> GetAllOrigins()
        {
            return _context.Origins.
                Where(o => !o.IsDeleted).
                ToList();
        }

        //Get Origin by Id
        public Origin? GetOriginById(int originId)
        {
            return _context.Origins.Find(originId);
        }

        // Add Origin
        public ServiceResult AddOrigin(Origin origin)
        {
            origin.Name = _utilitiesServices.CleanDataInput(origin.Name, false, false, true, true, true);

            if (string.IsNullOrWhiteSpace(origin.Name))
            {
                return new ServiceResult
                {
                    Success = false,
                    Error = "Tên xuất xứ không hợp lệ."
                };
            }

            bool isExist = _context.Origins.Any(o =>
                   o.Name.ToLower() == origin.Name.ToLower() &&
                   !o.IsDeleted
               );

            if (isExist) return new ServiceResult
            {
                Success = false,
                Error = "Tên xuất xứ đã tồn tại"
            };

            origin.IsActive = true;
            origin.IsDeleted = false;
            origin.CreatedAt = DateTime.Now;
            _context.Origins.Add(origin);
            _context.SaveChanges();
            return new ServiceResult
            {
                Success = true
            };
        }

        //Unit//---------------------------------------------

        //Get All Units
        public List<Unit> GetAllUnits()
        {
            return _context.Units.
                Where(u => !u.IsDeleted).
                ToList();
        }

        //Get Unit by Id
        public Unit? GetUnitById(int unitId)
        {
            return _context.Units.Find(unitId);
        }

        // Add Unit
        public ServiceResult AddUnit(Unit unit)
        {
            unit.Name = _utilitiesServices.CleanDataInput(unit.Name, false, false, true, true, true);
            if (string.IsNullOrWhiteSpace(unit.Name))
            {
                return new ServiceResult
                {
                    Success = false,
                    Error = "Tên đơn vị không hợp lệ."
                };
            }
            bool isExist = _context.Units.Any(u =>
                   u.Name.ToLower() == unit.Name.ToLower() &&
                   !u.IsDeleted
               );
            if (isExist) return new ServiceResult
            {
                Success = false,
                Error = "Tên đơn vị đã tồn tại"
            };
            unit.IsActive = true;
            unit.IsDeleted = false;
            unit.CreatedAt = DateTime.Now;
            _context.Units.Add(unit);
            _context.SaveChanges();
            return new ServiceResult
            {
                Success = true
            };
        }

        //Product//---------------------------------------------

        public bool IsProductSkuExist(string sku)
        {
            return _context.Products.Any(p => p.Sku.ToLower() == sku.ToLower() && !p.IsDeleted);
        }

        public bool IsProductNameExist(string name)
        {
            return _context.Products.Any(p => p.Name.ToLower() == name.ToLower() && !p.IsDeleted);
        }

        public bool IsPriceValid(decimal price)
        {
            return price >= 0;
        }

        public bool IsStockValid(int stock)
        {
            return stock >= 0;
        }

        //Get All Products
        public List<Product> GetAllProducts()
        {
            return _context.Products
                .Where(p => !p.IsDeleted).
                Include(p => p.Category).
                Include(p => p.Origin).
                Include(p => p.Unit).
                Include(p => p.ProductImages)
                .ToList();
        }

        //Get Product by Id
        public Product? GetProductById(int productId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Origin)
                .Include(p => p.Unit)
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == productId && !p.IsDeleted);
        }

        // Add Product
        public ServiceResult AddProduct(Product product, ICollection<ProductImage> images)
        {
            product.Name = _utilitiesServices.CleanDataInput(product.Name, false, false, true, true, true) ?? "";
            product.Sku = _utilitiesServices.CleanDataInput(product.Sku, false, false, true, true, true) ?? "";
            product.Description = _utilitiesServices.CleanDataInput(product.Description, false, false, true, true, true) ?? "";

            if (IsProductNameExist(product.Name))
            {
                return new ServiceResult
                {
                    Success = false,
                    Error = "Tên sản phẩm đã tồn tại."
                };
            }

            if (IsProductSkuExist(product.Sku))
            {
                return new ServiceResult
                {
                    Success = false,
                    Error = "SKU sản phẩm đã tồn tại."
                };
            }

            if (!IsPriceValid(product.Price))
            {
                return new ServiceResult
                {
                    Success = false,
                    Error = "Giá sản phẩm không hợp lệ."
                };
            }   

            if (!IsStockValid(product.Stock))
            {
                return new ServiceResult
                {
                    Success = false,
                    Error = "Số lượng tồn kho không hợp lệ."
                };
            }

            product.Status = 1;
            product.IsActive = true;
            product.IsDeleted = false;  
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            product.ProductImages = images;
            _context.Products.Add(product);
            _context.SaveChanges();
            // Implement product addition logic here
            return new ServiceResult
            {
                Success = true
            };

        }

        //Filter Products
        public List<Product> FilterProductsPaging(
         string? keyword,
         int? categoryId,
         int? originId,
         int? unitId,
         int? status,
         int pageIndex,
         int pageSize,
         out int totalItems)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Origin)
                .Include(p => p.Unit)
                .Include(p => p.ProductImages)
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.Name.Contains(keyword) || p.Sku.Contains(keyword));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (originId.HasValue)
                query = query.Where(p => p.OriginId == originId);

            if (unitId.HasValue)
                query = query.Where(p => p.UnitId == unitId);

            if (status.HasValue)
                query = query.Where(p => p.Status == status);

            totalItems = query.Count();

            return query
                .OrderByDescending(p => p.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        //Delete Product
        public ServiceResult DeleteProduct(int productId)
        {
            var product = _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == productId && !p.IsDeleted);
            if (product == null)
            {
                return new ServiceResult
                {
                    Success = false,
                    Error = "Sản phẩm không tồn tại."
                };
            }
            // Xoá ảnh sản phẩm khỏi hệ thống file
            foreach (var image in product.ProductImages)
            {
                _imageServices.DeleteImage(image.ImageUrl);
            }
            // Đánh dấu sản phẩm là đã xoá
            product.IsDeleted = true;
            product.Status = 3;
            product.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return new ServiceResult
            {
                Success = true
            };
        }

        public class ProductStatisticDto
        {
            public int Total { get; set; }
            public int Selling { get; set; }
            public int OutOfStock { get; set; }
            public int StopSelling { get; set; }
        }

        public ProductStatisticDto GetProductStatistics()
        {
            var query = _context.Products.Where(p => !p.IsDeleted);

            return new ProductStatisticDto
            {
                Total = query.Count(),
                Selling = query.Count(p => p.Status == 1),
                OutOfStock = query.Count(p => p.Status == 2),
                StopSelling = query.Count(p => p.Status == 3)
            };
        }

        //Update Product
        public ServiceResult UpdateProduct(Product editProduct, ICollection<ProductImage> newImages)
        {
            if (editProduct == null)
            {
                return new ServiceResult
                {
                    Success = false,
                    Error = "Dữ liệu sản phẩm không hợp lệ."
                };
            }

            var product = _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == editProduct.Id && !p.IsDeleted);

            if (product == null)
            {
                return new ServiceResult
                {
                    Success = false,
                    Error = "Sản phẩm không tồn tại."
                };
            }

            // Update thông tin cơ bản
            product.Name = editProduct.Name;
            product.Sku = editProduct.Sku;
            product.Description = editProduct.Description;
            product.Price = editProduct.Price;
            product.Stock = editProduct.Stock;
            product.CategoryId = editProduct.CategoryId;
            product.OriginId = editProduct.OriginId;
            product.UnitId = editProduct.UnitId;
            product.UpdatedAt = DateTime.Now;

            // Nếu có ảnh mới
            if (newImages != null && newImages.Any())
            {
                // Xoá ảnh cũ khỏi file + DB
                foreach (var img in product.ProductImages)
                {
                    _imageServices.DeleteImage(img.ImageUrl);
                }

                _context.ProductImages.RemoveRange(product.ProductImages);
                product.ProductImages.Clear();


                // Gán ảnh mới
                foreach (var img in newImages)
                {
                    img.ProductId = product.Id;
                    product.ProductImages.Add(img);
                }

            }

            _context.SaveChanges();

            return new ServiceResult { Success = true };
        }

        //Update Status
        public void UpdateStatus(Product product_status)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == product_status.Id && !p.IsDeleted);

            if (product == null) return;

            product.Status = product_status.Status;
            product.UpdatedAt = DateTime.Now;    

            _context.SaveChanges();
        }

        //Image

        public void UpdateMainImage(Product product)
        {
            if (product == null) return;
            product.UpdatedAt = DateTime.Now;
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        //WEBSITE/////////////////////////////////////////////////////
        public List<Product> FilterShopProducts(
            string? keyword,
            int? categoryId,
            int? originId,
            decimal? minPrice,
            decimal? maxPrice,
            int pageIndex,
            int pageSize,
            out int totalItems)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Origin)
                .Include(p => p.Unit)
                .Include(p => p.ProductImages)
                .Where(p => !p.IsDeleted && p.IsActive && p.Status == 1);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.Name.Contains(keyword));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (originId.HasValue)
                query = query.Where(p => p.OriginId == originId);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            totalItems = query.Count();

            return query
                .OrderByDescending(p => p.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

    }
}
