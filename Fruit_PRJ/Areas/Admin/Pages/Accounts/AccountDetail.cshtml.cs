using Fruit_PRJ.Models;
using Fruit_PRJ.Services;
using Fruit_Store_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_PRJ.Areas.Admin.Pages.Accounts
{
    public class AccountDetailModel : PageModel
    {
        private readonly AccountServices _accountServices;
        private readonly UtilitiesServices _utilitiesServices;

        public Account? account {  get; set; }
        public string Message { get; set; }

        public AccountDetailModel(AccountServices accountServices, UtilitiesServices utilitiesServices)
        {
            _accountServices = accountServices;
            _utilitiesServices = utilitiesServices;
        }

        public IActionResult OnGet(int? id)
        {
            CheckLogin();
            if (!id.HasValue)
                return RedirectToPage("Index");

            account = _accountServices.GetAccountById(id.Value);
            NewAccount = account;
            AccountId = id.Value;  

            if (account == null)
                return RedirectToPage("Index");

            Message = TempData["Message"] as string;

            return Page();
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

        public IActionResult OnPostToggleStatus(int id)
        {
            var result = _accountServices.ToggleAccountStatus(id);

            if (!result.Success)
            {
                TempData["Message"] = result.Error;
                return RedirectToPage(new { id });
            }

            TempData["Message"] = "Cập nhật trạng thái thành công";
            return RedirectToPage(new { id });
        }


        [BindProperty]
        public Account NewAccount { get; set; }

        public IActionResult OnPostUpdateProfile()
        {
            if (NewAccount == null)
            {
                TempData["Message"] = "Dữ liệu không hợp lệ.";
                return RedirectToPage("Index");
            }

            var result = _accountServices.UpdateAccountProfile(NewAccount);

            if (!result.Success)
            {
                TempData["Message"] = result.Error;
                return RedirectToPage(new { id = NewAccount.Id });
            }

            TempData["Message"] = "Cập nhật thông tin thành công";
            return RedirectToPage(new { id = NewAccount.Id });
        }


        [BindProperty]
        public string NewPassword { get; set; }
        [BindProperty]
        public int AccountId { get; set; }


        public IActionResult OnPostResetPassword()
        {
            if (AccountId <= 0)
            {
                TempData["Message"] = "Không xác định được tài khoản.";
                return RedirectToPage("Index");
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                TempData["Message"] = "Mật khẩu không được rỗng";
                return RedirectToPage(new { id = AccountId });
            }

            var result = _accountServices.ResetPassword(AccountId, NewPassword);

            TempData["Message"] = result.Success
                ? "Đổi mật khẩu thành công"
                : result.Error;

            return RedirectToPage(new { id = AccountId });
        }

        public void CheckLogin()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập trước.";
                Response.Redirect("/Admin/LoginAdmin");
                return;
            }

            if (HttpContext.Session.GetInt32("AdminRole") != 1)
            {
                TempData["Error"] = "Bạn không có quyền truy cập vào Account.";
                Response.Redirect("/Admin");
                return;
            }
        }





    }
}
