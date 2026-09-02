using AuctionMVC.Contracts;
using AuctionMVC.Services.Api;
using AuctionMVC.ViewModels.Bids;

namespace AuctionMVC.Services;

public interface IBidManagementService
{
Task<BidIndexViewModel> GetIndexAsync(string? auctionId, string? search, CancellationToken ct = default);
}

public class BidManagementService : IBidManagementService
{
    private readonly IBidsApiClient _bidsApiClient;
    private readonly IUsersApiClient _usersApiClient;
    private readonly IAuctionsApiClient _auctionsApiClient;
    private readonly IProductsApiClient _productsApiClient;

    public BidManagementService(
        IBidsApiClient bidsApiClient,
        IUsersApiClient usersApiClient,
        IAuctionsApiClient auctionsApiClient,
        IProductsApiClient productsApiClient)
    {
        _bidsApiClient = bidsApiClient;
        _usersApiClient = usersApiClient;
        _auctionsApiClient = auctionsApiClient;
        _productsApiClient = productsApiClient;
    }

    public async Task<BidIndexViewModel> GetIndexAsync(string? auctionId, string? search, CancellationToken ct = default)
    {
        var bidsTask = _bidsApiClient.GetAllAsync(ct);
        var usersTask = _usersApiClient.GetAllAsync(ct);
        var auctionsTask = _auctionsApiClient.GetAllAsync(ct);
        var productsTask = _productsApiClient.GetAllAsync(ct);
        await Task.WhenAll(bidsTask, usersTask, auctionsTask, productsTask);

        var bids = bidsTask.Result;
        var users = usersTask.Result;
        var auctions = auctionsTask.Result;
        var products = productsTask.Result;

        var userById = users.ToDictionary(u => u.Id, u => u);
        var productById = products.ToDictionary(p => p.Id, p => p);
        var auctionById = auctions.ToDictionary(a => a.Id, a => a);

var query = bids.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(auctionId) && Guid.TryParse(auctionId.Trim(), out var parsedAuctionId))
        {
            query = query.Where(b => b.AuctionId == parsedAuctionId);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(b =>
                (userById.TryGetValue(b.UserId, out var u) &&
                 (u.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                  u.Email.Contains(term, StringComparison.OrdinalIgnoreCase))) ||
                (auctionById.TryGetValue(b.AuctionId, out var a) &&
                 productById.TryGetValue(a.ProductId, out var p) &&
                 p.Name.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        var items = query
            .OrderByDescending(b => b.CreatedAt)
            .Select(b =>
            {
                var auction = auctionById.TryGetValue(b.AuctionId, out var a) ? a : null;
                var productName = auction is not null && productById.TryGetValue(auction.ProductId, out var p) ? p.Name : "—";

                return new BidListItemViewModel
                {
                    Id = b.Id,
                    AuctionId = b.AuctionId,
                    ProductName = productName,
                    BidderName = userById.TryGetValue(b.UserId, out var u) ? u.Name : "—",
                    BidderEmail = userById.TryGetValue(b.UserId, out var ue) ? ue.Email : "—",
                    Amount = b.Amount,
                    CreatedAt = b.CreatedAt,
                    AuctionStatus = auction?.Status ?? AuctionStatus.Pending
                };
            })
            .ToList();

        var totalAmount = bids.Sum(b => b.Amount);

return new BidIndexViewModel
        {
            Items = items,
            AuctionId = auctionId,
            Search = search,
            TotalBids = bids.Count,
            TotalAmount = totalAmount
        };
    }
}

