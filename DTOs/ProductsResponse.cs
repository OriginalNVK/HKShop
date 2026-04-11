using HKShop.Models;

namespace HKShop.DTOs
{
    public class ProductsResponse
    {
        public int MaHh { get; set; }

        public string TenHh { get; set; } = null!;

        public string? TenAlias { get; set; }

        public int MaLoai { get; set; }

        public string? MoTaDonVi { get; set; }

        public decimal? DonGia { get; set; }

        public string? Hinh { get; set; }

        public DateOnly NgaySx { get; set; }

        public decimal GiamGia { get; set; }

        public int LuotMua { get; set; }

        public string? MoTa { get; set; }
        public virtual Loai? MaLoaiNavigation { get; set; }
    }
}
