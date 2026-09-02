using AuctionMVC.ViewModels.Dashboard;

namespace AuctionMVC.Services;

public interface IDashboardService
{
    Task<DashboardIndexViewModel> GetDashboardAsync(CancellationToken ct = default);
}

