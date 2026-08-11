using AuctionMVC.Contracts;
using AuctionMVC.Services.Api;
using AuctionMVC.ViewModels.Dashboard;

namespace AuctionMVC.Services;

/// <summary>
/// Aggregates auction/product/bid/user data from the API into the dashboard VM.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IAuctionsApiClient _auctionsApiClient;
    private readonly IProductsApiClient _productsApiClient;
    private readonly IBidsApiClient _bidsApiClient;
    private readonly IUsersApiClient _usersApiClient;
    private readonly IWinnersApiClient _winnersApiClient;

    public DashboardService(
        IAuctionsApiClient auctionsApiClient,
        IProductsApiClient productsApiClient,
        IBidsApiClient bidsApiClient,
        IUsersApiClient usersApiClient,
        IWinnersApiClient winnersApiClient)
    {
        _auctionsApiClient = auctionsApiClient;
        _productsApiClient = productsApiClient;
        _bidsApiClient = bidsApiClient;
        _usersApiClient = usersApiClient;
        _winnersApiClient = winnersApiClient;
    }

    public async Task<DashboardIndexViewModel> GetDashboardAsync(CancellationToken ct = default)
    {
        // Fire all reads in parallel.
        var auctionsTask = _auctionsApiClient.GetAllAsync(ct);
        var productsTask = _productsApiClient.GetAllAsync(ct);
        var bidsTask = _bidsApiClient.GetAllAsync(ct);
        var usersTask = _usersApiClient.GetAllAsync(ct);
        var winnersTask = _winnersApiClient.GetAllAsync(ct);

        await Task.WhenAll(auctionsTask, productsTask, bidsTask, usersTask, winnersTask);

        var auctions = auctionsTask.Result;
        var products = productsTask.Result;
        var bids = bidsTask.Result;
        var users = usersTask.Result;
        var winners = winnersTask.Result;

        var productById = products.ToDictionary(p => p.Id, p => p);

        var activeCount = auctions.Count(a => a.Status == AuctionStatus.Active);
        var pendingCount = auctions.Count(a => a.Status == AuctionStatus.Pending);
        var finishedCount = auctions.Count(a => a.Status == AuctionStatus.Finished);
        var stoppedCount = auctions.Count(a => a.Status == AuctionStatus.Stopped);

        var now = DateTime.UtcNow;
        var recentAuctions = auctions
            .OrderByDescending(a => a.StartDate)
            .Take(6)
            .Select(a => new DashboardRecentAuctionViewModel
            {
                Id = a.Id,
                ProductName = productById.TryGetValue(a.ProductId, out var p) ? p.Name : "—",
                CurrentPrice = a.CurrentPrice,
                BidCount = bids.Count(b => b.AuctionId == a.Id),
                Status = a.Status,
                EndDate = a.EndDate
            })
            .ToList();

        var topAuction = auctions
            .Where(a => a.Status == AuctionStatus.Active)
            .OrderByDescending(a => a.CurrentPrice)
            .FirstOrDefault();

        DashboardTopAuctionViewModel? top = null;
        if (topAuction is not null)
        {
            top = new DashboardTopAuctionViewModel
            {
                Id = topAuction.Id,
                ProductName = productById.TryGetValue(topAuction.ProductId, out var p) ? p.Name : "—",
                CurrentPrice = topAuction.CurrentPrice,
                BidCount = bids.Count(b => b.AuctionId == topAuction.Id),
                EndDate = topAuction.EndDate
            };
        }

        // Monthly series (last 6 months) for the dashboard chart.
        var monthly = Enumerable.Range(5, 6)
            .Select(i => now.AddMonths(-i))
            .Select(m => new { m.Year, m.Month, Start = new DateTime(m.Year, m.Month, 1), End = new DateTime(m.Year, m.Month, 1).AddMonths(1) })
            .Select(x => new DashboardMonthlyPointViewModel
            {
                Label = x.Start.ToString("MMM"),
                Count = auctions.Count(a => a.StartDate >= x.Start && a.StartDate < x.End)
            })
            .ToList();

        return new DashboardIndexViewModel
        {
            TotalAuctions = auctions.Count,
            ActiveAuctions = activeCount,
            PendingAuctions = pendingCount,
            FinishedAuctions = finishedCount,
            StoppedAuctions = stoppedCount,
            TotalBids = bids.Count,
            TotalUsers = users.Count,
            TotalProducts = products.Count,
            TotalWinners = winners.Count,
            RecentAuctions = recentAuctions,
            TopAuction = top,
            MonthlySeries = monthly
        };
    }
}

