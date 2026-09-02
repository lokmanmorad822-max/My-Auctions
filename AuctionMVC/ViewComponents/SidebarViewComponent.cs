using Microsoft.AspNetCore.Mvc;

namespace AuctionMVC.ViewComponents;

/// <summary>Renders the admin sidebar navigation (DashShell aside).</summary>
public class SidebarViewComponent : ViewComponent
{
public IViewComponentResult Invoke(string? activeController = null)
    {
        // Explicitly specify the view name "Default" so that the string
        // parameter is treated as the model, not a view name lookup.
        return View("Default", activeController);
    }
}

