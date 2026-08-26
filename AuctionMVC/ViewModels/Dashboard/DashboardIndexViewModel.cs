using AuctionMVC.Contracts;

namespace AuctionMVC.ViewModels.Dashboard;

public class DashboardIndexViewModel
{
    public int TotalAuctions { get; set; }
    public int ActiveAuctions { get; set; }
    public int PendingAuctions { get; set; }
    public int FinishedAuctions { get; set; }
    public int StoppedAuctions { get; set; }
    public int TotalBids { get; set; }
    public int TotalUsers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalWinners { get; set; }

    public IReadOnlyList<DashboardRecentAuctionViewModel> RecentAuctions { get; set; }
        = Array.Empty<DashboardRecentAuctionViewModel>();

    public DashboardTopAuctionViewModel? TopAuction { get; set; }

    public IReadOnlyList<DashboardMonthlyPointViewModel> MonthlySeries { get; set; }
        = Array.Empty<DashboardMonthlyPointViewModel>();
}

public class DashboardRecentAuctionViewModel
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public int BidCount { get; set; }
    public AuctionStatus Status { get; set; }
    public DateTime EndDate { get; set; }
}

public class DashboardTopAuctionViewModel
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public int BidCount { get; set; }
    public DateTime EndDate { get; set; }
}

public class DashboardMonthlyPointViewModel
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}

