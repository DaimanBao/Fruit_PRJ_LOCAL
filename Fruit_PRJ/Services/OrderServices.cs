using Fruit_PRJ.Models;
using Fruit_Store_PRJ.Services;
using Microsoft.EntityFrameworkCore;

namespace Fruit_PRJ.Services
{
     public class CartViewModel
     {
         public Product Product { get; set; }
         public int Quantity { get; set; }
         public decimal SubTotal { get; set; }
     }

    public class OrderServices
    {
        private readonly FruitStoreDbContext _context;
        private readonly UtilitiesServices _utilities;

        public OrderServices(FruitStoreDbContext context, UtilitiesServices utilities)
        {
            _context = context;
            _utilities = utilities;
        }

        public class OrderResult
        {
            public bool Success { get; set; }
            public string? Error { get; set; }
            public Order? Order { get; set; }
        }

        // ============================

        public OrderResult CreateOrder(Order order, List<CartViewModel> cartItems)
        {
            if (order == null || !cartItems.Any())
                return new OrderResult { Success = false, Error = "Dữ liệu không hợp lệ" };

            // Bắt đầu một Transaction để đảm bảo nếu trừ kho lỗi thì đơn hàng không được tạo
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                order.CustomerName = _utilities.CleanDataInput(order.CustomerName);
                order.CustomerPhone = _utilities.CleanDataInput(order.CustomerPhone);
                order.ShippingAddress = _utilities.CleanDataInput(order.ShippingAddress);
                order.Note = _utilities.CleanDataInput(order.Note);

                order.OrderCode = _utilities.GenerateOrderCode();
                order.OrderDate = DateTime.Now;
                order.CreatedAt = DateTime.Now;
                order.Status = 1; // 1: Chờ xử lý

                order.TotalQuantity = cartItems.Sum(x => x.Quantity);
                order.TotalAmount = cartItems.Sum(x => x.SubTotal);

                foreach (var item in cartItems)
                {
                    var product = _context.Products.Find(item.Product.Id);

                    // Nếu không đủ hàng, phải Rollback rồi mới Return
                    if (product == null || product.Stock < item.Quantity)
                    {
                        transaction.Rollback(); // Đảm bảo mọi thứ quay lại ban đầu
                        return new OrderResult
                        {
                            Success = false,
                            Error = $"Sản phẩm {item.Product.Name} không đủ hàng (Còn lại: {product?.Stock ?? 0})"
                        };
                    }

                    product.Stock -= item.Quantity;
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = item.Product.Id,
                        ProductName = item.Product.Name,
                        UnitName = item.Product.Unit.Name,
                        Price = item.Product.Price,
                        Quantity = item.Quantity,
                        SubTotal = item.SubTotal
                    });
                }

                _context.Orders.Add(order);
                _context.SaveChanges();

                transaction.Commit(); // Hoàn tất giao dịch

                return new OrderResult { Success = true, Order = order };
            }
            catch (Exception ex)
            {
                transaction.Rollback(); // Nếu lỗi thì trả lại dữ liệu cũ
                return new OrderResult { Success = false, Error = "Lỗi hệ thống: " + ex.Message };
            }
        }

        // ============================

        public List<CartViewModel> BuildCartFromSession(List<CartItem> cart)
        {
            var result = new List<CartViewModel>();

            foreach (var c in cart)
            {
                var p = _context.Products
                    .Include(x => x.Unit)
                    .FirstOrDefault(x => x.Id == c.ProductId && !x.IsDeleted);

                if (p == null) continue;

                result.Add(new CartViewModel
                {
                    Product = p,
                    Quantity = c.Quantity,
                    SubTotal = p.Price * c.Quantity
                });
            }

            return result;
        }

        public decimal CalculateTotal(List<CartViewModel> cart)
            => cart.Sum(x => x.SubTotal);

        public void UpdatePaymentStatus(string orderCode, int status)
        {
            var order = _context.Orders.FirstOrDefault(x => x.OrderCode == orderCode);
            if (order != null)
            {
                order.Status = status; 
                _context.SaveChanges();
            }
        }
    }

}
