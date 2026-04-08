namespace ECommerce.Application.DTOs
{
    public class DashboardStatsDto
    {
        public int DailyViews { get; set; }
        public int DailyFavorites { get; set; }
        public int DailyCartAdditions { get; set; }
        public decimal DailyRevenue { get; set; }
        public int DailyOrdersCount { get; set; }
        public decimal WeeklyRevenue { get; set; }
        public int WeeklyOrdersCount { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int MonthlyOrdersCount { get; set; }
    }
}
