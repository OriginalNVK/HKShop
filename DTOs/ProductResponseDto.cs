namespace HKShop.DTOs;

public class ProductResponseDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? AliasName { get; set; }
    public int CategoryId { get; set; }
    public string? UnitDescription { get; set; }
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }
    public DateOnly ManufactureDate { get; set; }
    public decimal Discount { get; set; }
    public int Views { get; set; }
    public string? Description { get; set; }
    public object? Category { get; set; }
}
