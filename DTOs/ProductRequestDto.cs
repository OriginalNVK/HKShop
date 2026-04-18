using System.ComponentModel.DataAnnotations;
namespace HKShop.DTOs;

public class ProductRequestDto
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Product Name is required")]
    public string ProductName { get; set; } = string.Empty;

    public string? AliasName { get; set; }

    [Required(ErrorMessage = "Please select a product category")]
    public int? CategoryId { get; set; }

    public string? UnitDescription { get; set; }

    public decimal? Price { get; set; }

    public IFormFile? ImageFile { get; set; }

    [Required(ErrorMessage = "Production Date is required")]
    public DateTime ManufactureDate { get; set; }

    [Range(0, 100)]
    public decimal? Discount { get; set; }

    public int? Views { get; set; }

    public string? Description { get; set; }

    public object? Category { get; set; }
}
