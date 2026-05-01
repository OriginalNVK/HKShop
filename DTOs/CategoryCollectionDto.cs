namespace HKShop.DTOs{
    public class CategoryCollectionDto
    {
        public List<CategoryProducts> CategoryGroups { get; set; } = new();
    }

    public class CategoryProducts
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public List<ProductResponseDto> ProductItems { get; set; } = new();

        public int MaLoai
        {
            get => CategoryId;
            set => CategoryId = value;
        }

        public string TenLoai
        {
            get => CategoryName;
            set => CategoryName = value;
        }

        public List<ProductResponseDto> Products
        {
            get => ProductItems;
            set => ProductItems = value;
        }
    }
}
