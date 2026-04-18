namespace HKShop.DTOs{
    public class CategoryCollectionDto
    {
        public List<CategoryProducts> CategoryGroups { get; set; } = new();
    }

    public class CategoryProducts
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public List<ProductDto> ProductItems { get; set; } = new();
    }
}
