using AuctionMVC.Contracts;
using AuctionMVC.Options;
using Microsoft.Extensions.Options;

namespace AuctionMVC.Services.Api;

public interface IBidsApiClient
{
    Task<List<BidDto>> GetAllAsync(CancellationToken ct = default);
    Task<BidDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BidDto> CreateAsync(CreateBidDto dto, CancellationToken ct = default);
}

/// <summary>Typed client for <c>/api/bids</c>.</summary>
public class BidsApiClient : ApiClientBase, IBidsApiClient
{
    public BidsApiClient(
        HttpClient httpClient,
        IOptions<ApiOptions> apiOptions,
        IHttpContextAccessor httpContextAccessor)
        : base(httpClient, apiOptions, httpContextAccessor)
    {
    }

    private const string Resource = "/api/bids";

    public Task<List<BidDto>> GetAllAsync(CancellationToken ct = default)
        => GetAsync<List<BidDto>>(Resource, ct);

    public Task<BidDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        => GetAsync<BidDto>($"{Resource}/{id}", ct);

    public Task<BidDto> CreateAsync(CreateBidDto dto, CancellationToken ct = default)
        => SendAsync<BidDto>(HttpMethod.Post, Resource, dto, ct);
}

