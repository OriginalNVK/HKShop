namespace HKShop.DTOs
{
    public class GioHangModel
    {
        public int Quantity { get; set; }

        public decimal Total { get; set; }

        public List<GioHangItem> Items { get; set; }

    }
}
