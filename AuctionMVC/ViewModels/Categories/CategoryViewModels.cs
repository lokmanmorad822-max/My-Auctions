namespace AuctionMVC.ViewModels.Categories;

public class CategoryIndexViewModel
{
    public IReadOnlyList<CategoryListItemViewModel> Items { get; set; }
        = Array.Empty<CategoryListItemViewModel>();

    public int TotalCategories { get; set; }
}

public class CategoryListItemViewModel
{
    public string Name { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public int AuctionCount { get; set; }
}

