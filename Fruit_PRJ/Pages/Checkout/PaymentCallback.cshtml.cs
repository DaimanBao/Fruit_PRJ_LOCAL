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
            if (string.IsNullOrEmpty(code))
            {
                return RedirectToPage("/Index");
            }

            if (status == "success")
            {
                // Cập nhật trạng thái đơn hàng là "Đã thanh toán" (Ví dụ status = 2)
                _orderServices.UpdatePaymentStatus(code, 2);

                // Chuyển về trang thông báo thành công
                return RedirectToPage("/Checkout/Success", new { code = code });
            }
            else
            {
                return RedirectToPage("/Cart/Index", new { message = "Thanh toán không thành công. Vui lòng thử lại." });
            }
        }
    }
}
