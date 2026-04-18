namespace HKShop.DTOs;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? CategoryAlias { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}
