using System.ComponentModel.DataAnnotations;

namespace AuctionMVC.ViewModels.Products;

public class ProductIndexViewModel
{
    public IReadOnlyList<ProductListItemViewModel> Items { get; set; }
        = Array.Empty<ProductListItemViewModel>();

    public IReadOnlyList<string> Categories { get; set; }
        = Array.Empty<string>();

    public string CurrentCategory { get; set; } = "all";
    public string? Search { get; set; }
}

public class ProductListItemViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Images { get; set; } = string.Empty;
    public int AuctionCount { get; set; }
}

public class ProductFormViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "اسم المنتج مطلوب.")]
    [MaxLength(200, ErrorMessage = "اسم المنتج يجب ألا يتجاوز 200 حرف.")]
    [Display(Name = "اسم المنتج")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "الوصف يجب ألا يتجاوز 2000 حرف.")]
    [Display(Name = "الوصف")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "التصنيف مطلوب.")]
    [MaxLength(100, ErrorMessage = "التصنيف يجب ألا يتجاوز 100 حرف.")]
    [Display(Name = "التصنيف")]
    public string Category { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "روابط الصور يجب ألا تتجاوز 1000 حرف.")]
    [Display(Name = "روابط الصور (مفصولة بفاصلة)")]
    public string? Images { get; set; }

    public IReadOnlyList<string> AvailableCategories { get; set; }
        = Array.Empty<string>();
}

