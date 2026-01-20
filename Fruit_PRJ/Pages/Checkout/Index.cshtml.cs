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

        public void OnGet()
        {
            LoadCart();
        }

        public IActionResult OnPost()
        {
            LoadCart();

            if (!CartItems.Any())
                return RedirectToPage("/Cart/Index");

            var orderResult = _orderServices.CreateOrder(Order, CartItems);

            if (!orderResult.Success || orderResult.Order == null)
            {
                ModelState.AddModelError("", orderResult.Error ?? "Lỗi tạo đơn hàng");
                return Page();
            }

            
            HttpContext.Session.Remove("CART");

            if (Order.PaymentMethod == 3) // STRIPE
            {
                var domain = $"{Request.Scheme}://{Request.Host}";

                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"/Checkout/PaymentCallback?code={orderResult.Order.OrderCode}&status=success",
                    CancelUrl = domain + $"/Checkout/PaymentCallback?code={orderResult.Order.OrderCode}&status=cancel",
                };

                
                foreach (var item in CartItems)
                {
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)item.Product.Price, 
                            Currency = "vnd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                            },
                        },
                        Quantity = item.Quantity,
                    });
                }

                var service = new SessionService();
                Session session = service.Create(options);

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return RedirectToPage("/Checkout/Success", new
            {
                code = orderResult.Order.OrderCode,
                payMethod = orderResult.Order.PaymentMethod
            });
        }

        public void LoadCart()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("CART") ?? new();
            CartItems = _orderServices.BuildCartFromSession(cart);
            TotalAmount = _orderServices.CalculateTotal(CartItems);
        }
    }
}
