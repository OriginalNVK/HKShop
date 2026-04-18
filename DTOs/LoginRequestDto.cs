using System.ComponentModel.DataAnnotations;

namespace HKShop.DTOs;

public class LoginRequestDto
{
    [Display(Name = "Username")]
    [Required(ErrorMessage = "Username is required")]
    [MaxLength(20, ErrorMessage = "Username must be at most 20 characters")]
    public string Username { get; set; } = null!;

    [Display(Name = "Password")]
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}

