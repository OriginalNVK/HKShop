using System.ComponentModel.DataAnnotations;

namespace HKShop.DTOs
{
    public class DangKyRequest
    {
        [Key]
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        public string TenDangNhap { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public string? MatKhau { get; set; }

        [Display(Name = "Họ tên")]
        [MaxLength(50, ErrorMessage = "Tối đa là 50 ký tự")]
        public string HoTen { get; set; } = null!;

        public bool GioiTinh { get; set; } = true;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateOnly? NgaySinh { get; set; }

        [MaxLength(60, ErrorMessage = "Tối đa là 60 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [MaxLength(24, ErrorMessage = "Tối đa là 24 ký tự")]
        [RegularExpression(@"0\d{9}", ErrorMessage = "Sai định dạng SĐT VN")]
        [Display(Name = "Điện thoại")]
        public string DienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Sai định dạng email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string? Hinh { get; set; }
    }
}
