using AuctionMVC.Contracts;
using AuctionMVC.Services.Api;
using AuctionMVC.ViewModels.Users;

namespace AuctionMVC.Services;

public interface IUserManagementService
{
    Task<UserIndexViewModel> GetIndexAsync(string? search, CancellationToken ct = default);
    Task<UserFormViewModel> GetCreateModelAsync();
    Task<UserFormViewModel> GetEditModelAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(UserFormViewModel model, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UserFormViewModel model, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public class UserManagementService : IUserManagementService
{
    private readonly IUsersApiClient _usersApiClient;
    private readonly IAuctionsApiClient _auctionsApiClient;
    private readonly IBidsApiClient _bidsApiClient;
    private readonly IWinnersApiClient _winnersApiClient;

    public UserManagementService(
        IUsersApiClient usersApiClient,
        IAuctionsApiClient auctionsApiClient,
        IBidsApiClient bidsApiClient,
        IWinnersApiClient winnersApiClient)
    {
        _usersApiClient = usersApiClient;
        _auctionsApiClient = auctionsApiClient;
        _bidsApiClient = bidsApiClient;
        _winnersApiClient = winnersApiClient;
    }

    public async Task<UserIndexViewModel> GetIndexAsync(string? search, CancellationToken ct = default)
    {
        var usersTask = _usersApiClient.GetAllAsync(ct);
        var auctionsTask = _auctionsApiClient.GetAllAsync(ct);
        var bidsTask = _bidsApiClient.GetAllAsync(ct);
        var winnersTask = _winnersApiClient.GetAllAsync(ct);
        await Task.WhenAll(usersTask, auctionsTask, bidsTask, winnersTask);

        var users = usersTask.Result;
        var auctions = auctionsTask.Result;
        var bids = bidsTask.Result;
        var winners = winnersTask.Result;

        var query = users.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(u =>
                u.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                u.Phone.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        var items = query
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserListItemViewModel
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Phone = u.Phone,
                CreatedAt = u.CreatedAt,
                AuctionCount = auctions.Count(a => a.UserId == u.Id),
                BidCount = bids.Count(b => b.UserId == u.Id),
                WinCount = winners.Count(w => w.UserId == u.Id)
            })
            .ToList();

        return new UserIndexViewModel
        {
            Items = items,
            Search = search,
            TotalUsers = users.Count
        };
    }

    public Task<UserFormViewModel> GetCreateModelAsync()
        => Task.FromResult(new UserFormViewModel());

public async Task<UserFormViewModel> GetEditModelAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _usersApiClient.GetByIdAsync(id, ct);

        return new UserFormViewModel
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone ?? string.Empty,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<Guid> CreateAsync(UserFormViewModel model, CancellationToken ct = default)
    {
        // TODO(BACKEND): The API accepts PasswordHash directly. In production this
        // should be computed by the backend from a plain-text password via a
        // dedicated register/change-password endpoint.
        var created = await _usersApiClient.CreateAsync(new CreateUserDto
        {
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone,
PasswordHash = model.Password ?? string.Empty
        }, ct);

        return created.Id;
    }

    public async Task UpdateAsync(Guid id, UserFormViewModel model, CancellationToken ct = default)
    {
        await _usersApiClient.UpdateAsync(id, new UpdateUserDto
        {
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone
        }, ct);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => _usersApiClient.DeleteAsync(id, ct);
}

