using AuctionMVC.Contracts;
using AuctionMVC.Services.Api;
using AuctionMVC.ViewModels.Categories;

namespace AuctionMVC.Services;

public interface ICategoryService
{
    Task<CategoryIndexViewModel> GetIndexAsync(CancellationToken ct = default);
}

/// <summary>
/// Categories are derived from the Product.Category values returned by the API.
/// A dedicated category CRUD endpoint does not yet exist in the backend —
/// see TODO(BACKEND) markers.
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly IProductsApiClient _productsApiClient;
    private readonly IAuctionsApiClient _auctionsApiClient;

    public CategoryService(
        IProductsApiClient productsApiClient,
        IAuctionsApiClient auctionsApiClient)
    {
        _productsApiClient = productsApiClient;
        _auctionsApiClient = auctionsApiClient;
    }

    public async Task<CategoryIndexViewModel> GetIndexAsync(CancellationToken ct = default)
    {
        var productsTask = _productsApiClient.GetAllAsync(ct);
        var auctionsTask = _auctionsApiClient.GetAllAsync(ct);
        await Task.WhenAll(productsTask, auctionsTask);

        var products = productsTask.Result;
        var auctions = auctionsTask.Result;

        var groups = products
            .Where(p => !string.IsNullOrWhiteSpace(p.Category))
            .GroupBy(p => p.Category, StringComparer.OrdinalIgnoreCase)
            .Select(g => new CategoryListItemViewModel
            {
                Name = g.Key,
                ProductCount = g.Count(),
                AuctionCount = g.Count(p => auctions.Any(a => a.ProductId == p.Id))
            })
            .OrderBy(c => c.Name)
            .ToList();

        return new CategoryIndexViewModel
        {
            Items = groups,
            TotalCategories = groups.Count
        };
    }
}

