namespace HKShop.DTOs
{
    public class GioHangItem
    {
        public int MaHH { get; set; }

        public string Hinh { get; set; } = null!;

        public string TenHH { get; set; } = null!;

        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }

        public decimal ThanhTien => SoLuong * DonGia;
    }
}
