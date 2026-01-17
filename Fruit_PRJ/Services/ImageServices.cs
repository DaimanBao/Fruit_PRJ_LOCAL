using Fruit_PRJ.Models;

namespace Fruit_Store_PRJ.Services
{
    public class ImageServices
    {
        private readonly string _rootImagePath = Path.Combine("wwwroot", "uploads", "products");
        public ImageServices()
        {
            if(!Directory.Exists(_rootImagePath))
            {
                Directory.CreateDirectory(_rootImagePath);
            }
        }

        public List<ProductImage> SaveProductImages(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                throw new Exception("Chưa chọn ảnh sản phẩm");

            var images = new List<ProductImage>();
            bool isMain = true;

            foreach (var file in files)
            {
                var ext = Path.GetExtension(file.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowed.Contains(ext))
                    throw new Exception("Ảnh không hợp lệ");

                var fileName = $"{Guid.NewGuid()}{ext}";
                var fullPath = Path.Combine(_rootImagePath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                images.Add(new ProductImage
                {
                    ImageUrl = $"/uploads/products/{fileName}",
                    IsMain = isMain
                });

                isMain = false;
            }

            return images;
        }


        public void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return;

            var relativePath = imageUrl.TrimStart('/')
                .Replace("/", Path.DirectorySeparatorChar.ToString());

            var fullPath = Path.Combine("wwwroot", relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

    }
}
