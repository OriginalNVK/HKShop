namespace HKShop.DTOs;

public class ProductCardViewModel
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public decimal? Price { get; set; }

    public decimal Discount { get; set; }

    public string? ImageUrl { get; set; }

    public bool ShowNewBadge { get; set; } = true;

    public bool CompactRating { get; set; }

    public decimal FinalPrice => (Price ?? 0) * (1 - Discount / 100m);
}
