using AuctionMVC.Contracts;
using AuctionMVC.Options;
using Microsoft.Extensions.Options;

namespace AuctionMVC.Services.Api;

public interface IProductsApiClient
{
    Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

/// <summary>Typed client for <c>/api/products</c>.</summary>
public class ProductsApiClient : ApiClientBase, IProductsApiClient
{
    public ProductsApiClient(
        HttpClient httpClient,
        IOptions<ApiOptions> apiOptions,
        IHttpContextAccessor httpContextAccessor)
        : base(httpClient, apiOptions, httpContextAccessor)
    {
    }

    private const string Resource = "/api/products";

    public Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default)
        => GetAsync<List<ProductDto>>(Resource, ct);

    public Task<ProductDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        => GetAsync<ProductDto>($"{Resource}/{id}", ct);

    public Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
        => SendAsync<ProductDto>(HttpMethod.Post, Resource, dto, ct);

    public Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken ct = default)
        => SendAsync<ProductDto>(HttpMethod.Put, $"{Resource}/{id}", dto, ct);

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        using var response = await DeleteAsync($"{Resource}/{id}", ct);
        if (!response.IsSuccessStatusCode)
        {
            throw await Exceptions.ApiException.FromResponseAsync(response, await response.Content.ReadAsStringAsync(ct));
        }
    }
}

