namespace HKShop.DTOs;

public class ProductDetailDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public decimal Price { get; set; }
    public string ShortDescription { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int RatingScore { get; set; }
    public int StockQuantity { get; set; }
}
