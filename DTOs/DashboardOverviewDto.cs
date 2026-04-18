namespace HKShop.DTOs;

public class DashboardOverviewDto
{
    public List<DailyMetricDto> CustomerIn7Day { get; set; } = new();
    public List<DailyMetricDto> OrderIn14Day { get; set; } = new();
}

public class DailyMetricDto 
{
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public int Amount { get; set; }
}
