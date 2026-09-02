using AuctionMVC.Contracts;
using AuctionMVC.Services.Api;
using AuctionMVC.ViewModels.Auctions;

namespace AuctionMVC.Services;

public interface IAuctionManagementService
{
    Task<AuctionIndexViewModel> GetIndexAsync(string? status, string? search, CancellationToken ct = default);
    Task<AuctionDetailsViewModel> GetDetailsAsync(Guid id, CancellationToken ct = default);
    Task<AuctionFormViewModel> GetCreateModelAsync(CancellationToken ct = default);
    Task<AuctionFormViewModel> GetEditModelAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(AuctionFormViewModel model, CancellationToken ct = default);
    Task UpdateAsync(Guid id, AuctionFormViewModel model, CancellationToken ct = default);
    Task ApproveAsync(Guid id, CancellationToken ct = default);
    Task RejectAsync(Guid id, CancellationToken ct = default);
    Task StopAsync(Guid id, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public class AuctionManagementService : IAuctionManagementService
{
    private readonly IAuctionsApiClient _auctionsApiClient;
    private readonly IProductsApiClient _productsApiClient;
    private readonly IBidsApiClient _bidsApiClient;
    private readonly IUsersApiClient _usersApiClient;
    private readonly IWinnersApiClient _winnersApiClient;

    public AuctionManagementService(
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

    public async Task<AuctionIndexViewModel> GetIndexAsync(string? status, string? search, CancellationToken ct = default)
    {
        var auctionsTask = _auctionsApiClient.GetAllAsync(ct);
        var productsTask = _productsApiClient.GetAllAsync(ct);
        var usersTask = _usersApiClient.GetAllAsync(ct);
        var bidsTask = _bidsApiClient.GetAllAsync(ct);

        await Task.WhenAll(auctionsTask, productsTask, bidsTask, usersTask);

        var auctions = auctionsTask.Result;
        var products = productsTask.Result;
        var users = usersTask.Result;
        var bids = bidsTask.Result;

        var productById = products.ToDictionary(p => p.Id, p => p);
        var userById = users.ToDictionary(u => u.Id, u => u);

        var query = auctions.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(status) && status != "all")
        {
            var parsed = Enum.TryParse<AuctionStatus>(status, true, out var s);
            if (parsed)
            {
                query = query.Where(a => a.Status == s);
            }
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(a =>
                (productById.TryGetValue(a.ProductId, out var p) &&
                 (p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                  p.Category.Contains(term, StringComparison.OrdinalIgnoreCase))) ||
                (userById.TryGetValue(a.UserId, out var u) &&
                 u.Name.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        var items = query
            .OrderByDescending(a => a.StartDate)
            .Select(a => new AuctionListItemViewModel
            {
                Id = a.Id,
                ProductName = productById.TryGetValue(a.ProductId, out var p) ? p.Name : "—",
                Category = productById.TryGetValue(a.ProductId, out var pc) ? pc.Category : "—",
                OwnerName = userById.TryGetValue(a.UserId, out var u) ? u.Name : "—",
                StartPrice = a.StartPrice,
                CurrentPrice = a.CurrentPrice,
                EndDate = a.EndDate,
                BidCount = bids.Count(b => b.AuctionId == a.Id),
                Status = a.Status
            })
            .ToList();

        var counts = new AuctionStatusCountsViewModel
        {
            All = auctions.Count,
            Pending = auctions.Count(a => a.Status == AuctionStatus.Pending),
            Active = auctions.Count(a => a.Status == AuctionStatus.Active),
            Finished = auctions.Count(a => a.Status == AuctionStatus.Finished),
            Stopped = auctions.Count(a => a.Status == AuctionStatus.Stopped),
            Rejected = auctions.Count(a => a.Status == AuctionStatus.Rejected)
        };

        return new AuctionIndexViewModel
        {
            Items = items,
            Counts = counts,
            CurrentStatus = status ?? "all",
            Search = search
        };
    }

    public async Task<AuctionDetailsViewModel> GetDetailsAsync(Guid id, CancellationToken ct = default)
    {
        var auction = await _auctionsApiClient.GetByIdAsync(id, ct);
        var productTask = _productsApiClient.GetByIdAsync(auction.ProductId, ct);
        var userTask = _usersApiClient.GetByIdAsync(auction.UserId, ct);
        var bidsTask = _bidsApiClient.GetAllAsync(ct);

        await Task.WhenAll(productTask, userTask, bidsTask);

        var product = productTask.Result;
        var owner = userTask.Result;
        var bids = bidsTask.Result
            .Where(b => b.AuctionId == id)
            .OrderByDescending(b => b.Amount)
            .ToList();

        var users = await _usersApiClient.GetAllAsync(ct);
        var userById = users.ToDictionary(u => u.Id, u => u);

        // Winner lookup (may not exist)
        WinnerDto? winner = null;
        try
        {
            var winners = await _winnersApiClient.GetAllAsync(ct);
            winner = winners.FirstOrDefault(w => w.AuctionId == id);
        }
        catch
        {
            // Winner lookup is best-effort.
        }

        return new AuctionDetailsViewModel
        {
            Id = auction.Id,
            ProductId = auction.ProductId,
            ProductName = product?.Name ?? "—",
            Category = product?.Category ?? "—",
            Description = product?.Description ?? string.Empty,
            Images = product?.Images ?? string.Empty,
            OwnerName = owner?.Name ?? "—",
            StartPrice = auction.StartPrice,
            CurrentPrice = auction.CurrentPrice,
            StartDate = auction.StartDate,
            EndDate = auction.EndDate,
            Status = auction.Status,
            BidCount = bids.Count,
            Bids = bids.Select(b => new AuctionBidViewModel
            {
                Id = b.Id,
                BidderName = userById.TryGetValue(b.UserId, out var u) ? u.Name : "—",
                Amount = b.Amount,
                CreatedAt = b.CreatedAt
            }).ToList(),
            WinnerFinalPrice = winner?.FinalPrice,
            WinnerUserName = winner is not null && userById.TryGetValue(winner.UserId, out var wu) ? wu.Name : null
        };
    }

    public async Task<AuctionFormViewModel> GetCreateModelAsync(CancellationToken ct = default)
    {
        var products = await _productsApiClient.GetAllAsync(ct);
        var users = await _usersApiClient.GetAllAsync(ct);

        return new AuctionFormViewModel
        {
            StartDate = DateTime.UtcNow.AddHours(1),
            EndDate = DateTime.UtcNow.AddDays(7),
            Status = AuctionStatus.Pending,
            AvailableProducts = products,
            AvailableUsers = users
        };
    }

    public async Task<AuctionFormViewModel> GetEditModelAsync(Guid id, CancellationToken ct = default)
    {
        var auction = await _auctionsApiClient.GetByIdAsync(id, ct);
        var products = await _productsApiClient.GetAllAsync(ct);
        var users = await _usersApiClient.GetAllAsync(ct);

        return new AuctionFormViewModel
        {
            Id = auction.Id,
            UserId = auction.UserId,
            ProductId = auction.ProductId,
            StartPrice = auction.StartPrice,
            CurrentPrice = auction.CurrentPrice,
            StartDate = auction.StartDate,
            EndDate = auction.EndDate,
            Status = auction.Status,
            AvailableProducts = products,
            AvailableUsers = users
        };
    }

    public async Task<Guid> CreateAsync(AuctionFormViewModel model, CancellationToken ct = default)
    {
        var created = await _auctionsApiClient.CreateAsync(new CreateAuctionDto
        {
            UserId = model.UserId,
            ProductId = model.ProductId,
            StartPrice = model.StartPrice,
            CurrentPrice = model.CurrentPrice,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Status = model.Status
        }, ct);

        return created.Id;
    }

    public async Task UpdateAsync(Guid id, AuctionFormViewModel model, CancellationToken ct = default)
    {
        await _auctionsApiClient.UpdateAsync(id, new UpdateAuctionDto
        {
            StartPrice = model.StartPrice,
            CurrentPrice = model.CurrentPrice,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Status = model.Status
        }, ct);
    }

    public Task ApproveAsync(Guid id, CancellationToken ct = default)
        => _auctionsApiClient.ApproveAsync(id, ct);

    public Task RejectAsync(Guid id, CancellationToken ct = default)
        => _auctionsApiClient.RejectAsync(id, ct);

    public Task StopAsync(Guid id, CancellationToken ct = default)
        => _auctionsApiClient.StopAsync(id, ct);

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => _auctionsApiClient.DeleteAsync(id, ct);
}

