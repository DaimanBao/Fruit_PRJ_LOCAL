using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_PRJ.Pages.Checkout
{
    public class PaymentCallbackModel : PageModel
    {
        private readonly OrderServices _orderServices;

        public PaymentCallbackModel(OrderServices orderServices)
        {
            _orderServices = orderServices;
        }

        public IActionResult OnGet(string code, string status)
        {
            if (string.IsNullOrEmpty(code)) return RedirectToPage("/Index");

            var order = _orderServices.GetOrderByCode(code, HttpContext.Session.GetInt32("AccountId") ?? 0); 
            if (order == null) return RedirectToPage("/Index");

        
            if (order.Status >= 2)
            {
                return RedirectToPage("/Checkout/Success", new { code = code });
            }


            if (status == "success")
            {
                _orderServices.UpdatePaymentStatus(code, 2);

                HttpContext.Session.Remove("Cart");

                return RedirectToPage("/Checkout/Success", new { code = code });
            }
            else
            {
                return RedirectToPage("/Cart/Index", new { error = "Thanh toán thất bại" });
            }
        }
    }
}
