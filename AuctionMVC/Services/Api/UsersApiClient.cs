using AuctionMVC.Contracts;
using AuctionMVC.Options;
using Microsoft.Extensions.Options;

namespace AuctionMVC.Services.Api;

public interface IUsersApiClient
{
    Task<List<UserDto>> GetAllAsync(CancellationToken ct = default);
    Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken ct = default);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

/// <summary>Typed client for <c>/api/users</c>.</summary>
public class UsersApiClient : ApiClientBase, IUsersApiClient
{
    public UsersApiClient(
        HttpClient httpClient,
        IOptions<ApiOptions> apiOptions,
        IHttpContextAccessor httpContextAccessor)
        : base(httpClient, apiOptions, httpContextAccessor)
    {
    }

    private const string Resource = "/api/users";

    public Task<List<UserDto>> GetAllAsync(CancellationToken ct = default)
        => GetAsync<List<UserDto>>(Resource, ct);

    public Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        => GetAsync<UserDto>($"{Resource}/{id}", ct);

    public Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken ct = default)
        => SendAsync<UserDto>(HttpMethod.Post, Resource, dto, ct);

    public Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken ct = default)
        => SendAsync<UserDto>(HttpMethod.Put, $"{Resource}/{id}", dto, ct);

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        using var response = await DeleteAsync($"{Resource}/{id}", ct);
        if (!response.IsSuccessStatusCode)
        {
            throw await Exceptions.ApiException.FromResponseAsync(response, await response.Content.ReadAsStringAsync(ct));
        }
    }
}

