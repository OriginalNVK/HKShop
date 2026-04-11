namespace HKShop.DTOs
{
    public class CategoryResponse
    {
        public int MaLoai { get; set; }

        public string TenLoai { get; set; } = null!;

        public string? TenLoaiAlias { get; set; }

        public string? MoTa { get; set; }

        public string? Hinh { get; set; }
    }
}
