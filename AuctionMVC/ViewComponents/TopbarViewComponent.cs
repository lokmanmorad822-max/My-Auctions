using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace AuctionMVC.ViewComponents;

public class TopbarModel
{
    public string PageTitle { get; set; } = string.Empty;
    public string UserName { get; set; } = "مدير النظام";
    public string UserInitials { get; set; } = "م";
    public string? UserEmail { get; set; }
}

/// <summary>Renders the admin topbar (page title, search, user dropdown).</summary>
public class TopbarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string? title = null)
    {
        var user = ViewContext.HttpContext.User;
        var name = user.Identity?.IsAuthenticated == true
            ? user.FindFirstValue(ClaimTypes.Name) ?? "مدير النظام"
            : "مدير النظام";

        var email = user.Identity?.IsAuthenticated == true
            ? user.FindFirstValue(ClaimTypes.Email)
            : null;

        var initials = string.IsNullOrWhiteSpace(name)
            ? "م"
            : new string(name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Take(2)
                .Select(w => w[0])
                .ToArray());

        var model = new TopbarModel
        {
            PageTitle = title ?? "لوحة التحكم",
            UserName = name,
            UserInitials = initials,
            UserEmail = email
        };

        return View(model);
    }
}

