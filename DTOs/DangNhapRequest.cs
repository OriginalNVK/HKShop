using System.ComponentModel.DataAnnotations;

namespace HKShop.DTOs
{
    public class DangNhapRequest
    {
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Chưa nhập tên đăng nhập")]
        [MaxLength(20, ErrorMessage = "Tên đăng nhập tối đa 20 kí tự")]
        public string Username { get; set; } = null!;

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Chưa nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
