using AuctionMVC.Contracts;
using AuctionMVC.Options;
using Microsoft.Extensions.Options;

namespace AuctionMVC.Services.Api;

public interface IWinnersApiClient
{
    Task<List<WinnerDto>> GetAllAsync(CancellationToken ct = default);
    Task<WinnerDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<WinnerDto> CreateAsync(CreateWinnerDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

/// <summary>Typed client for <c>/api/winners</c>.</summary>
public class WinnersApiClient : ApiClientBase, IWinnersApiClient
{
    public WinnersApiClient(
        HttpClient httpClient,
        IOptions<ApiOptions> apiOptions,
        IHttpContextAccessor httpContextAccessor)
        : base(httpClient, apiOptions, httpContextAccessor)
    {
    }

    private const string Resource = "/api/winners";

    public Task<List<WinnerDto>> GetAllAsync(CancellationToken ct = default)
        => GetAsync<List<WinnerDto>>(Resource, ct);

    public Task<WinnerDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        => GetAsync<WinnerDto>($"{Resource}/{id}", ct);

    public Task<WinnerDto> CreateAsync(CreateWinnerDto dto, CancellationToken ct = default)
        => SendAsync<WinnerDto>(HttpMethod.Post, Resource, dto, ct);

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        using var response = await DeleteAsync($"{Resource}/{id}", ct);
        if (!response.IsSuccessStatusCode)
        {
            throw await Exceptions.ApiException.FromResponseAsync(response, await response.Content.ReadAsStringAsync(ct));
        }
    }
}

