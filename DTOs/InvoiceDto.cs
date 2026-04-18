namespace HKShop.DTOs;

public class InvoiceDto
{
    public int InvoiceId { get; set; }
    public string CustomerName { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public string Address { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public string ShippingMethod { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Notes { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = null!;
}
