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



        //NEW ACCOUNT
        public List<Account> Accounts { get; set; } = new List<Account>();

        [BindProperty]
        public Account NewAccount { get; set; } = new();

        [BindProperty]
        public string RePass { get; set; }

        //Mess
        public string Message { get; set; }

        //Search
        [BindProperty(SupportsGet = true)]
        public string? SearchKeyword { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? FilterRole {  get; set; }
        [BindProperty(SupportsGet = true)]
        public bool? FilterStatus { get; set; }

        //Pagination

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalAccounts { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalAccounts / PageSize);
        
        //TOTAL
        public int TotalAccount {  get; set; }
        public int TotalActive {  get; set; }
        public int TotalInactive { get; set; }
        public int TotalEmployees { get; set; }



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

            Accounts = _accountServices.FilterAccountPaging(
                SearchKeyword,
                FilterRole,
                FilterStatus,
                PageIndex,
                PageSize,
                out int total);
            TotalAccounts = total;

            var stat = _accountServices.GetAccountStatistic();
            TotalAccount = stat.Total;
            TotalActive = stat.Active;
            TotalInactive = stat.Inactive;
            TotalEmployees = stat.Employees;
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
                OnLoad();
                return Page();
            }

            if(NewAccount.PasswordHash != RePass)
            {
                Message = "Repass và password không trùng nhau";
                OnLoad();
                return Page();
            }

            var result = _accountServices.AddAccountAdminPanel(NewAccount);

            if (!result.Success)
            {
                Message = result.Error;
                OnLoad();
                return Page();
            }
            OnLoad();
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

        public IActionResult OnPostToggleStatus(int id)
        {
            var result = _accountServices.ToggleAccountStatus(id);

            if (!result.Success)
            {
                TempData["Message"] = result.Error;
                return RedirectToPage();
            }

            TempData["Message"] = "Cập nhật trạng thái thành công";
            return RedirectToPage();
        }

    }
}
