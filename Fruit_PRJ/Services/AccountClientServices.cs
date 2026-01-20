using Fruit_PRJ.Models;
using Fruit_Store_PRJ.Services;
using static Fruit_PRJ.Services.AccountServices;

namespace Fruit_PRJ.Services
{
    public class AccountClientServices
    {
        private readonly FruitStoreDbContext _dbContext;
        private readonly UtilitiesServices _utilities;

        public AccountClientServices(FruitStoreDbContext dbContext, UtilitiesServices utilities)
        {
            _dbContext = dbContext;
            _utilities = utilities;
        }

        public ServiceResult RegisterCustomer(Account model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.PasswordHash))
                return new ServiceResult { Success = false, Error = "Thông tin không đầy đủ." };

            if (_dbContext.Accounts.Any(a => a.Email == model.Email))
                return new ServiceResult { Success = false, Error = "Email này đã được đăng ký." };

            model.Username = _utilities.CleanDataInput(model.Username, false, false, true, true, true);
            model.Role = 3; 
            model.IsActive = true;
            model.CreatedAt = DateTime.Now;
            model.PasswordHash = HashPassword(model.PasswordHash);

            _dbContext.Accounts.Add(model);
            _dbContext.SaveChanges();
            return new ServiceResult { Success = true };
        }

        public LoginResult LoginCustomer(string email, string password)
        {
            var acc = _dbContext.Accounts.FirstOrDefault(a => a.Email == email);

            if (acc == null)
                return new LoginResult { Success = false, Error = "Tài khoản không tồn tại." };

            if (!acc.IsActive)
                return new LoginResult { Success = false, Error = "Tài khoản của bạn đã bị khóa." };

            if (acc.PasswordHash != HashPassword(password))
                return new LoginResult { Success = false, Error = "Mật khẩu không chính xác." };

            return new LoginResult { Success = true, Account = acc };
        }

        public ServiceResult UpdateProfile(int userId, Account updateModel)
        {
            var user = _dbContext.Accounts.Find(userId);
            if (user == null) return new ServiceResult { Success = false, Error = "Không tìm thấy người dùng." };

            user.Username = _utilities.CleanDataInput(updateModel.Username, false, false, true, true, true);
            user.Phone = _utilities.CleanDataInput(updateModel.Phone, false, false, false, true, true);
            user.Address = _utilities.CleanDataInput(updateModel.Address, false, false, true, true, true);

            _dbContext.SaveChanges();
            return new ServiceResult { Success = true };
        }

        private string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
