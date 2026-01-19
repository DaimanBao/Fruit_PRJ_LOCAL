using Fruit_PRJ.Models;
using Fruit_PRJ.Services;
using Fruit_Store_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_PRJ.Areas.Admin.Pages.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly AccountServices _accountServices;
        private readonly UtilitiesServices _utilitiesServices;


        public List<Account> Accounts { get; set; } = new List<Account>();

        [BindProperty]
        public Account NewAccount { get; set; } = new();

        [BindProperty]
        public string RePass { get; set; }

        public string Message { get; set; }


        public IndexModel(AccountServices accountServices,UtilitiesServices utilitiesServices)
        {
            _accountServices = accountServices;
            _utilitiesServices = utilitiesServices;
        }

        public void OnLoad()
        {
            Accounts = _accountServices.GetAllAccounts();
        }


        public void OnGet()
        {
            Message = TempData["Message"] as string;
            OnLoad();
        }

        public IActionResult OnPostCreateAccountAdminPanel()
        {
            if (string.IsNullOrWhiteSpace(NewAccount.Username) ||
                string.IsNullOrWhiteSpace(NewAccount.Phone) ||
                string.IsNullOrWhiteSpace(NewAccount.Email)||
                string.IsNullOrWhiteSpace(NewAccount.PasswordHash)||
                string.IsNullOrWhiteSpace(RePass))
            {
                Message = "Thông tin không được bỏ trống";
                return Page();
            }

            if(NewAccount.PasswordHash != RePass)
            {
                Message = "Repass và password không trùng nhau";
                return Page();
            }

            var result = _accountServices.AddAccountAdminPanel(NewAccount);

            if (!result.Success)
            {
                Message = result.Error;
                return Page();
            }
            TempData["Message"] = "Thêm tài khoản thành công";
            return RedirectToPage();
        }

        public string GetAccountRoleText(int roleInt)
        {
            return _utilitiesServices.GetAccountRoleText(roleInt);
        }

        public string GetAccountRoleClass(int roleInt)
        {
            return _utilitiesServices.GetAccountRoleClass(roleInt);
        }

        public string GetAccountStatusText(bool statusInt)
        {
            return _utilitiesServices.GetAccountStatusText(statusInt);
        }

        public string GetAccountStatusClass(bool statusInt)
        {
            return _utilitiesServices.GetAccountStatusClass(statusInt);
        }
    }
}
