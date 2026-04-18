namespace HKShop.DTOs;

public class CheckoutRequestDto
{
    public bool UseCustomerProfile { get; set; }
    public string? FullName { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Notes { get; set; }
}
