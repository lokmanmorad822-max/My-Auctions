using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AuctionMVC.TagHelpers;

/// <summary>
/// Adds the Bootstrap "active" class to a sidebar link when its controller/area
/// matches the current route:
/// <code><a asp-controller="Auctions" active-route>...</a></code>
/// </summary>
[HtmlTargetElement("a", Attributes = "active-route")]
public class ActiveRouteTagHelper : TagHelper
{
    [HtmlAttributeName("asp-controller")]
    public string? Controller { get; set; }

    [HtmlAttributeName("asp-area")]
    public string? Area { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var routeValues = ViewContext.RouteData.Values;

        var currentController = routeValues["controller"]?.ToString() ?? string.Empty;
        var currentArea = routeValues["area"]?.ToString();

        var matchesController = string.IsNullOrEmpty(Controller) ||
                                string.Equals(currentController, Controller, StringComparison.OrdinalIgnoreCase);

        var matchesArea = string.IsNullOrEmpty(Area) ||
                          string.Equals(currentArea, Area, StringComparison.OrdinalIgnoreCase);

        if (matchesController && matchesArea)
        {
            var existing = output.Attributes.FirstOrDefault(a => a.Name == "class")?.Value?.ToString();
            output.Attributes.SetAttribute("class", $"{existing} active".Trim());
        }
    }
}

