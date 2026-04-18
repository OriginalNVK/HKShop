using System.ComponentModel.DataAnnotations;

namespace HKShop.DTOs;

public class RegisterRequestDto
{
    [Key]
    [Display(Name = "Username")]
    [Required(ErrorMessage = "*")]
    [MaxLength(20, ErrorMessage = "Lenght of 20 characters maximum")]
    public string Username { get; set; } = null!;

    [Display(Name = "Password")]
    [Required(ErrorMessage = "*")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Full Name")]
    [MaxLength(50, ErrorMessage = "Length of 50 characters maximum")]
    public string FullName { get; set; } = null!;

    public bool Gender { get; set; } = true;

    [Display(Name = "Birth Date")]
    [DataType(DataType.Date)]
    public DateOnly? BirthDate { get; set; }
    [MaxLength(60, ErrorMessage = "Length of 60 characters maximum")]

    [Display(Name = "Address")]
    public string Address { get; set; } = null!;
    
    [MaxLength(24, ErrorMessage = "Length of 24 characters maximum")]
    [RegularExpression(@"0\d{9}", ErrorMessage = "Invalid phone number format")]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = null!;
    
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;
    
    public string? ImageUrl { get; set; }
}
