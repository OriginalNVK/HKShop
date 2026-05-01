namespace HKShop.DTOs;

public class InvoiceDetailDto
{
    public int DetailInvoiceId { get; set; }
    public int InvoiceId { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public string ProductImage { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}
