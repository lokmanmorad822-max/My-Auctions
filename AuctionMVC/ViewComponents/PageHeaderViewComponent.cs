using Microsoft.AspNetCore.Mvc;

namespace AuctionMVC.ViewComponents;

public class PageHeaderModel
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? ActionLabel { get; set; }
    public string? ActionUrl { get; set; }
    public string? ActionIcon { get; set; }
}

/// <summary>
/// Renders a consistent page header (title + optional action button) used at the
/// top of every admin content page.
/// </summary>
public class PageHeaderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(PageHeaderModel model)
    {
        return View(model);
    }
}

