using Microsoft.AspNetCore.Mvc;

namespace AuctionMVC.ViewComponents;

public class StatusFilterItem
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Url { get; set; }
    public int Count { get; set; }
}

public class StatusFilterModel
{
    public IReadOnlyList<StatusFilterItem> Items { get; set; }
        = Array.Empty<StatusFilterItem>();
    public string CurrentValue { get; set; } = "all";
    public string? BaseUrl { get; set; }
}

/// <summary>Renders a row of filter pills (used on Auctions, Products pages).</summary>
public class StatusFilterViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(StatusFilterModel model)
    {
        return View(model);
    }
}

