using Fruit_PRJ.Extensions;
using Fruit_PRJ.Models;
using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe.Checkout;
using static Fruit_PRJ.Services.OrderServices;

namespace Fruit_PRJ.Pages.Checkout
{
    public class IndexModel : PageModel
    {
        private readonly OrderServices _orderServices;
        private readonly IConfiguration _configuration;

        public IndexModel(OrderServices orderServices, IConfiguration configuration)
        {
            _orderServices = orderServices;
            _configuration = configuration;
        }

        [BindProperty]
        public Order Order { get; set; } = new();

        public List<CartViewModel> CartItems { get; set; } = new();
        public decimal TotalAmount { get; set; }

        // THÊM DÒNG NÀY: Để lưu thông báo lỗi khi tạo đơn thất bại
        [TempData]
        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return RedirectToPage("/Login_Logout/Index", new { returnUrl = "/thanh-toan" });
            }

            LoadCart();

            if (!CartItems.Any()) return RedirectToPage("/Cart/Index");

            Order.CustomerName = HttpContext.Session.GetString("CustomerName") ?? "";
            Order.CustomerPhone = HttpContext.Session.GetString("CustomerPhone") ?? "";
            Order.ShippingAddress = HttpContext.Session.GetString("CustomerAddress") ?? "";

            return Page();
        }

        public IActionResult OnPost()
        {
            LoadCart();

            if (!CartItems.Any()) return RedirectToPage("/Cart/Index");

            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId.HasValue)
            {
                Order.AccountId = customerId.Value;
            }
            else
            {
                return RedirectToPage("/Login_Logout/SignIn_SignUp", new { returnUrl = "/thanh-toan" });
            }

            var orderResult = _orderServices.CreateOrder(Order, CartItems);

            if (!orderResult.Success || orderResult.Order == null)
            {
                ModelState.AddModelError(string.Empty, orderResult.Error ?? "Có lỗi xảy ra khi tạo đơn hàng.");
                return Page();
            }

            // Xóa giỏ hàng sau khi tạo đơn thành công
            HttpContext.Session.Remove("CART");
            HttpContext.Session.Remove("CartCount"); // Xóa cả số lượng hiển thị trên Header

            // Nếu thanh toán Stripe
            if (Order.PaymentMethod == 3)
            {
                return ProcessStripePayment(orderResult.Order);
            }

            // SỬA TẠI ĐÂY: Truyền thêm payMethod để trang Success hiển thị đúng thông tin
            return RedirectToPage("/Checkout/Success", new
            {
                code = orderResult.Order.OrderCode,
                payMethod = orderResult.Order.PaymentMethod
            });
        }
        private IActionResult ProcessStripePayment(Order order)
        {
            var domain = $"{Request.Scheme}://{Request.Host}";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"/Checkout/PaymentCallback?code={order.OrderCode}&status=success",
                CancelUrl = domain + $"/Checkout/PaymentCallback?code={order.OrderCode}&status=cancel",
                CustomerEmail = HttpContext.Session.GetString("CustomerEmail"),
            };

            foreach (var item in CartItems)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        // Lưu ý: Stripe tính theo đơn vị nhỏ nhất (với VND là đồng, không cần *100)
                        UnitAmount = (long)Math.Round(item.Product.Price, 0),
                        Currency = "vnd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions { Name = item.Product.Name },
                    },
                    Quantity = item.Quantity,
                });
            }

            var service = new SessionService();
            Session session = service.Create(options);
            return Redirect(session.Url);
        }

        public void LoadCart()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("CART") ?? new();
            CartItems = _orderServices.BuildCartFromSession(cart);
            TotalAmount = _orderServices.CalculateTotal(CartItems);
        }
    }
}
