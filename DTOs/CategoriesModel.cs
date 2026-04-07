namespace HKShop.DTOs
{
    public class CategoriesModel
    {
        public List<CategoryProducts> Categories { get; set; }
    }

    public class CategoryProducts
    {
        public int MaLoai { get; set; }
        public string TenLoai { get; set; }
        public List<HangHoaResponse> Products { get; set; }
    }
}
