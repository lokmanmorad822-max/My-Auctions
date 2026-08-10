using AuctionMVC.Contracts;
using AuctionMVC.Services.Api;
using AuctionMVC.ViewModels.Winners;

namespace AuctionMVC.Services;

public interface IWinnerManagementService
{
    Task<WinnerIndexViewModel> GetIndexAsync(string? search, CancellationToken ct = default);
    Task<Guid> CreateAsync(CreateWinnerViewModel model, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public class WinnerManagementService : IWinnerManagementService
{
    private readonly IWinnersApiClient _winnersApiClient;
    private readonly IUsersApiClient _usersApiClient;
    private readonly IAuctionsApiClient _auctionsApiClient;
    private readonly IProductsApiClient _productsApiClient;

    public WinnerManagementService(
        IWinnersApiClient winnersApiClient,
        IUsersApiClient usersApiClient,
        IAuctionsApiClient auctionsApiClient,
        IProductsApiClient productsApiClient)
    {
        _winnersApiClient = winnersApiClient;
        _usersApiClient = usersApiClient;
        _auctionsApiClient = auctionsApiClient;
        _productsApiClient = productsApiClient;
    }

    public async Task<WinnerIndexViewModel> GetIndexAsync(string? search, CancellationToken ct = default)
    {
        var winnersTask = _winnersApiClient.GetAllAsync(ct);
        var usersTask = _usersApiClient.GetAllAsync(ct);
        var auctionsTask = _auctionsApiClient.GetAllAsync(ct);
        var productsTask = _productsApiClient.GetAllAsync(ct);
        await Task.WhenAll(winnersTask, usersTask, auctionsTask, productsTask);

        var winners = winnersTask.Result;
        var users = usersTask.Result;
        var auctions = auctionsTask.Result;
        var products = productsTask.Result;

        var userById = users.ToDictionary(u => u.Id, u => u);
        var productById = products.ToDictionary(p => p.Id, p => p);
        var auctionById = auctions.ToDictionary(a => a.Id, a => a);

        var query = winners.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(w =>
                (userById.TryGetValue(w.UserId, out var u) &&
                 (u.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                  u.Email.Contains(term, StringComparison.OrdinalIgnoreCase))) ||
                (auctionById.TryGetValue(w.AuctionId, out var a) &&
                 productById.TryGetValue(a.ProductId, out var p) &&
                 p.Name.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        var items = query
            .OrderByDescending(w => auctionById.TryGetValue(w.AuctionId, out var a) ? a.EndDate : DateTime.MinValue)
            .Select(w =>
            {
                var auction = auctionById.TryGetValue(w.AuctionId, out var a) ? a : null;
                var productName = auction is not null && productById.TryGetValue(auction.ProductId, out var p) ? p.Name : "—";
                var winnerName = userById.TryGetValue(w.UserId, out var u) ? u.Name : "—";

                return new WinnerListItemViewModel
                {
                    Id = w.Id,
                    AuctionId = w.AuctionId,
                    ProductName = productName,
                    WinnerName = winnerName,
                    WinnerEmail = userById.TryGetValue(w.UserId, out var ue) ? ue.Email : "—",
                    FinalPrice = w.FinalPrice,
                    AuctionStatus = auction?.Status ?? AuctionStatus.Pending,
                    EndedAt = auction?.EndDate
                };
            })
            .ToList();

        var totalWinnings = winners.Sum(w => w.FinalPrice);

        return new WinnerIndexViewModel
        {
            Items = items,
            Search = search,
            TotalWinners = winners.Count,
            TotalWinnings = totalWinnings
        };
    }

    public async Task<Guid> CreateAsync(CreateWinnerViewModel model, CancellationToken ct = default)
    {
        var created = await _winnersApiClient.CreateAsync(new CreateWinnerDto
        {
            AuctionId = model.AuctionId,
            UserId = model.UserId,
            FinalPrice = model.FinalPrice
        }, ct);

        return created.Id;
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => _winnersApiClient.DeleteAsync(id, ct);
}

