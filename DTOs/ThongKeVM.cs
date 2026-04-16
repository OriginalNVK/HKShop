namespace HKShop.DTOs
{
    public class OverviewDTO
    {
        public List<CustomerOrOrderOverview> CustomerIn7Day { get; set; } = new();
        public List<CustomerOrOrderOverview> OrderIn14Day { get; set; } = new();
    }

    public class CustomerOrOrderOverview
    {
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public int Amount { get; set; }
    }
}
