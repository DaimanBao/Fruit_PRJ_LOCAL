using Fruit_PRJ.Models;
using Fruit_Store_PRJ.Services;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

namespace Fruit_PRJ.Services
{
    public class AccountServices
    {
        private readonly FruitStoreDbContext _dbContext;
        private readonly UtilitiesServices _utilitiesServices;


        public class ServiceResult
        {
            public bool Success { get; set; }
            public string? Error { get; set; }
        }

        public AccountServices(FruitStoreDbContext dbContext,UtilitiesServices utilitiesServices)
        {
            _dbContext = dbContext;
            _utilitiesServices = utilitiesServices;
        }

        
        //Add Account
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public bool IsNameExist(string accountName)
        {
            return _dbContext.Accounts.Any(ac => ac.Username.ToLower() == accountName.ToLower());
        }

        public bool IsEmailExist(string accountEmail)
        {
            return _dbContext.Accounts.Any(ac => ac.Email == accountEmail);
        }

        public bool IsPhoneExist(string accountPhone)
        {
            return _dbContext.Accounts.Any(a => a.Phone == accountPhone);
        }

        public bool CheckEmailValid(string accounteredEmail)
        {
            if (string.IsNullOrWhiteSpace(accounteredEmail))
                return false;

            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(accounteredEmail, pattern);
        }

        public ServiceResult AddAccountAdminPanel(Account accountAdminPanel)
        {
            if(accountAdminPanel == null) return new ServiceResult { Success = false, Error = "Dữ liệu không hợp lệ." };

            accountAdminPanel.Address ??= "Không có địa chỉ";

            accountAdminPanel.Address = _utilitiesServices.CleanDataInput(accountAdminPanel.Address, false,false,true,true,true);
            accountAdminPanel.Username = _utilitiesServices.CleanDataInput(accountAdminPanel.Username,false,false,true,true,true);
            accountAdminPanel.Email = _utilitiesServices.CleanDataInput(accountAdminPanel.Email, false, false, false, true, true);
            accountAdminPanel.Phone = _utilitiesServices.CleanDataInput(accountAdminPanel.Phone,false,false,false,true,true);

            if(IsNameExist(accountAdminPanel.Username)) return new ServiceResult { Success = false, Error = "Username đã tồn tại." };
            if (IsEmailExist(accountAdminPanel.Email)) return new ServiceResult { Success = false, Error = "Tên email đã tồn tại" };
            if(IsPhoneExist(accountAdminPanel.Phone)) return new ServiceResult { Success = false, Error = "Số điện thoại đã tồn tại." };
            if(!CheckEmailValid(accountAdminPanel.Email)) return new ServiceResult{Success = false,Error = "Email không đúng định dạng."};

            accountAdminPanel.PasswordHash = HashPassword(accountAdminPanel.PasswordHash);
            accountAdminPanel.CreatedAt = DateTime.Now;
            accountAdminPanel.IsActive = true;

            _dbContext.Accounts.Add(accountAdminPanel);
            _dbContext.SaveChanges();

            return new ServiceResult { Success = true };
        }

        public List<Account> GetAllAccounts()
        {
            return _dbContext.Accounts.ToList();
        }

    }
}
