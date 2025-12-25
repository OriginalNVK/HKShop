namespace HKShop.DTOs
{
    public class HangHoaResponse
    {
        public int MaHh { get; set; }

        public string TenHH { get; set; } = null!;

        public string Hinh { get; set; } = null!;

        public decimal DonGia { get; set; }

        public string MoTaNgan { get; set; } = null!;

        public string TenLoai { get; set; } = null!;

        public decimal GiamGia { get; set; }
    }
}
