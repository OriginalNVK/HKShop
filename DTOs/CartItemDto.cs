namespace HKShop.DTOs;

public class CartItemDto
{
    public int ProductId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => Quantity * Price;
}
