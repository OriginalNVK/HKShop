using System.Text;

namespace HKShop.Helpers
{
    public static class Utils
    {
        public static string GenerateRandomKey(int length = 5)
        {
            var pattern = @"qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM,./;'[]!@#$%^&*()";
            var sb = new StringBuilder();
            var rd = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }
            return sb.ToString();
        }

        public static string convertStatus(string status)
        {
            switch (status)
            {
                case "cancelled":
                    return "Đơn hàng đã bị hủy";
                case "pending":
                    return "Đơn hàng chưa được duyệt";
                case "processing":
                    return "Đơn hàng đang chờ giao";
                case "paid":
                    return "Đơn hàng đã được thanh toán";
                case "completed":
                    return "Đơn hàng đã hoàn thành";
                default:
                    return "Đơn hàng lỗi";
            }
        }

        public static string convertRole(int role)
        {
            switch (role)
            {
                case 0:
                    return "Người dùng";
                case 1:
                    return "Quản trị viên";
                default:
                    return "Lỗi";
            }
        }
    }
}
