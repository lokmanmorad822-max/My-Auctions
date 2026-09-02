using System.Net.Http.Json;
using AuctionMVC.Contracts;
using AuctionMVC.Options;

namespace AuctionMVC.Services.Api;

public interface IAuctionsApiClient
{
    Task<List<AuctionDto>> GetAllAsync(CancellationToken ct = default);
    Task<AuctionDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AuctionDto> CreateAsync(CreateAuctionDto dto, CancellationToken ct = default);
    Task<AuctionDto> UpdateAsync(Guid id, UpdateAuctionDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<AuctionDto> ApproveAsync(Guid id, CancellationToken ct = default);
    Task<AuctionDto> RejectAsync(Guid id, CancellationToken ct = default);
    Task<AuctionDto> StopAsync(Guid id, CancellationToken ct = default);
}

/// <summary>Typed client for <c>/api/auctions</c>.</summary>
public class AuctionsApiClient : ApiClientBase, IAuctionsApiClient
{
    public AuctionsApiClient(
        HttpClient httpClient,
        Microsoft.Extensions.Options.IOptions<ApiOptions> apiOptions,
        IHttpContextAccessor httpContextAccessor)
        : base(httpClient, apiOptions, httpContextAccessor)
    {
    }

    private const string Resource = "/api/auctions";

    public Task<List<AuctionDto>> GetAllAsync(CancellationToken ct = default)
        => GetAsync<List<AuctionDto>>(Resource, ct);

    public Task<AuctionDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        => GetAsync<AuctionDto>($"{Resource}/{id}", ct);

    public Task<AuctionDto> CreateAsync(CreateAuctionDto dto, CancellationToken ct = default)
        => SendAsync<AuctionDto>(HttpMethod.Post, Resource, dto, ct);

    public Task<AuctionDto> UpdateAsync(Guid id, UpdateAuctionDto dto, CancellationToken ct = default)
        => SendAsync<AuctionDto>(HttpMethod.Put, $"{Resource}/{id}", dto, ct);

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        using var response = await DeleteAsync($"{Resource}/{id}", ct);
        if (!response.IsSuccessStatusCode)
        {
            throw await Exceptions.ApiException.FromResponseAsync(response, await response.Content.ReadAsStringAsync(ct));
        }
    }

    public Task<AuctionDto> ApproveAsync(Guid id, CancellationToken ct = default)
        => SendAsync<AuctionDto>(HttpMethod.Post, $"{Resource}/{id}/approve", null, ct);

    public Task<AuctionDto> RejectAsync(Guid id, CancellationToken ct = default)
        => SendAsync<AuctionDto>(HttpMethod.Post, $"{Resource}/{id}/reject", null, ct);

    public Task<AuctionDto> StopAsync(Guid id, CancellationToken ct = default)
        => SendAsync<AuctionDto>(HttpMethod.Post, $"{Resource}/{id}/stop", null, ct);
}

