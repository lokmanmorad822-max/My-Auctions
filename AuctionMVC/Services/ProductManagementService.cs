using AuctionMVC.Contracts;
using AuctionMVC.Services.Api;
using AuctionMVC.ViewModels.Products;

namespace AuctionMVC.Services;

public interface IProductManagementService
{
    Task<ProductIndexViewModel> GetIndexAsync(string? search, string? category, CancellationToken ct = default);
    Task<ProductFormViewModel> GetCreateModelAsync(CancellationToken ct = default);
    Task<ProductFormViewModel> GetEditModelAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(ProductFormViewModel model, CancellationToken ct = default);
    Task UpdateAsync(Guid id, ProductFormViewModel model, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken ct = default);
}

public class ProductManagementService : IProductManagementService
{
    private readonly IProductsApiClient _productsApiClient;
    private readonly IAuctionsApiClient _auctionsApiClient;

    public ProductManagementService(
        IProductsApiClient productsApiClient,
        IAuctionsApiClient auctionsApiClient)
    {
        _productsApiClient = productsApiClient;
        _auctionsApiClient = auctionsApiClient;
    }

    public async Task<ProductIndexViewModel> GetIndexAsync(string? search, string? category, CancellationToken ct = default)
    {
        var productsTask = _productsApiClient.GetAllAsync(ct);
        var auctionsTask = _auctionsApiClient.GetAllAsync(ct);
        await Task.WhenAll(productsTask, auctionsTask);

        var products = productsTask.Result;
        var auctions = auctionsTask.Result;

        var query = products.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(category) && category != "all")
        {
            query = query.Where(p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p =>
                p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Category.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        var items = query
            .OrderBy(p => p.Name)
            .Select(p => new ProductListItemViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Description = p.Description,
                Images = p.Images,
                AuctionCount = auctions.Count(a => a.ProductId == p.Id)
            })
            .ToList();

        var categories = products
            .Select(p => p.Category)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c)
            .ToList();

        return new ProductIndexViewModel
        {
            Items = items,
            Categories = categories,
            CurrentCategory = category ?? "all",
            Search = search
        };
    }

    public async Task<ProductFormViewModel> GetCreateModelAsync(CancellationToken ct = default)
    {
        return new ProductFormViewModel
        {
            AvailableCategories = await GetCategoriesAsync(ct)
        };
    }

    public async Task<ProductFormViewModel> GetEditModelAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _productsApiClient.GetByIdAsync(id, ct);

        return new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            Images = product.Images,
            AvailableCategories = await GetCategoriesAsync(ct)
        };
    }

    public async Task<Guid> CreateAsync(ProductFormViewModel model, CancellationToken ct = default)
    {
        var created = await _productsApiClient.CreateAsync(new CreateProductDto
        {
            Name = model.Name,
            Description = model.Description ?? string.Empty,
            Category = model.Category,
            Images = model.Images ?? string.Empty
        }, ct);

        return created.Id;
    }

    public async Task UpdateAsync(Guid id, ProductFormViewModel model, CancellationToken ct = default)
    {
        await _productsApiClient.UpdateAsync(id, new UpdateProductDto
        {
            Name = model.Name,
            Description = model.Description ?? string.Empty,
            Category = model.Category,
            Images = model.Images ?? string.Empty
        }, ct);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => _productsApiClient.DeleteAsync(id, ct);

    public async Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken ct = default)
    {
        var products = await _productsApiClient.GetAllAsync(ct);
        return products
            .Select(p => p.Category)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c)
            .ToList();
    }
}

