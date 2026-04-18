namespace HKShop.DTOs;

public class CustomerRequestDto
{
    public string CustomerId { get; set; } = null!;
    public string? Password { get; set; }
    public string FullName { get; set; } = null!;
    public bool Gender { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string Email { get; set; } = null!;
    public int Role { get; set; }
    public string? ImageUrl { get; set; }
}
