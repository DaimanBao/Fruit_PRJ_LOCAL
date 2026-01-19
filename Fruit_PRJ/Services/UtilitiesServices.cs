using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Fruit_Store_PRJ.Services
{
    public class UtilitiesServices
    {



        // Hàm làm sạch dữ liệu đầu vào
        public string? CleanDataInput(
            string? inputData,
            bool toLower=false,
            bool removeDiacritics=false,
            bool removeSpecialChars=false,
            bool singleSpace=true,
            bool trim=true )
        {
            if(string.IsNullOrWhiteSpace(inputData))
                return null;

            string result = inputData;

            //  Trim đầu cuối
            if (trim)
                result = result.Trim();

            //  Chuẩn hoá xuống dòng, tab → space
            result = Regex.Replace(result, @"[\t\r\n]+", " ");

            //  Chuẩn hoá nhiều space → 1 space
            if (singleSpace)
                result = Regex.Replace(result, @"\s+", " ");

            //  Bỏ dấu tiếng Việt / Unicode
            if (removeDiacritics)
                result = RemoveDiacritics(result);

            //  Bỏ ký tự đặc biệt (giữ chữ + số + space)
            if (removeSpecialChars)
                result = Regex.Replace(result, @"[^\p{L}\p{N}\s]", "");

            //  Chuyển lowercase
            if (toLower)
                result = result.ToLowerInvariant();

            return string.IsNullOrWhiteSpace(result) ? null : result;
        }

        // Hàm phụ cho CleanDataInput: tác dụng bỏ dấu Unicode (Việt, Pháp, Đức…)
        private string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }



        // Ham chuyen id cua product sang text
        public string GetProductStatusText(int statusId)
        {
            switch (statusId)
            {
                case 1:
                    return "Đang bán";
                case 2:
                    return "Hết hàng";
                case 3:
                    return "Ngừng bán";
                default:
                    return "Unknown";
            }

        }

        // Lay ten class de dung trong style
        public string GetStatusClass(int status)
        {
            return status switch
            {
                1 => "success",
                2 => "danger",
                3 => "secondary",
                _ => "dark"
            };
        }

        // Ham sinh orderCode ngau nhien cho order
        public string GenerateOrderCode(string prefix = "OD")
        {
            var shortGuid = Guid.NewGuid().ToString("N")[..8].ToUpper();
            var datePart = DateTime.Now.ToString("yyyyMMdd");

            return $"{prefix}{datePart}-{shortGuid}";
        }

        // Ham sinh token cho reset password
        public string GenerateResetToken(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rnd = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }

        public string GetAccountStatusText(int statusId)
        {
            switch (statusId)
            {
                case 1:
                    return "Đang hoạt động";
                case 0:
                    return "Ngừng hoạt động";
                default:
                    return "Unknown";
            }

        }

        public string GetAccountStatusClass(bool status)
        {
            return status ? "success" : "secondary";
        }

        public string GetAccountStatusText(bool status)
        {
            return status ? "Hoạt động" : "Ngừng hoạt động";
        }

        public string GetAccountRoleText(int statusId)
        {
            switch (statusId)
            {
                case 1:
                    return "Admin";
                case 2:
                    return "Staff";
                case 3:
                    return "Customer";
                default:
                    return "Unknown";
            }

        }

        public string GetAccountRoleClass(int status)
        {
            return status switch
            {
                1 => "danger",
                2 => "primary",
                3 => "success",
                _ => "dark"
            };
        }
    }
}
