namespace HKShop.DTOs;

public class CartSummaryDto
{
    public int TotalQuantity { get; set; }
    public decimal Total { get; set; }
    public List<CartItemDto> CartItems { get; set; } = new();
}
