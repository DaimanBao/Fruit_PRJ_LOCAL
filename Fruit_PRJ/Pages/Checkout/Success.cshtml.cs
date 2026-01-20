using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_PRJ.Pages.Checkout
{
    public class SuccessModel : PageModel
    {
        public string OrderCode { get; set; } = string.Empty;
        public int PaymentMethod { get; set; } 

        public void OnGet(string code, int payMethod)
        {
            OrderCode = code;
            PaymentMethod = payMethod;
        }
    }
}
