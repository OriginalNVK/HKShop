namespace HKShop.DTOs
{
    public class ChiTietHangHoaResponse
    {
        public int MaHH { get; set; }

        public string TenHH { get; set; } = null!;

        public string Hinh { get; set; } = null!;

        public decimal DonGia { get; set; }

        public string MoTaNgan { get; set; } = null!;

        public string TenLoai { get; set; } = null!;

        public string ChiTiet { get; set; } = null!;

        public int DiemDanhGia { get; set; }

        public int SoLuongTon { get; set; }
    }
}
