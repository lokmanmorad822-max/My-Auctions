using Microsoft.AspNetCore.Mvc;

namespace AuctionMVC.ViewComponents;

public class StatCardModel
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Hint { get; set; } = string.Empty;
    public string Icon { get; set; } = "bi-box";
    public string IconBg { get; set; } = "bg-soft-success";
    public string IconColor { get; set; } = "text-primary";
    public string HintColor { get; set; } = "text-primary";
}

/// <summary>Renders a dashboard stat card matching the design's stat block.</summary>
public class StatCardViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(StatCardModel model)
    {
        return View(model);
    }
}

