using System.Globalization;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AuctionMVC.TagHelpers;

/// <summary>
/// Formats a decimal as an Arabic (ar-SA) currency amount:
/// <code><currency amount="42500" /></code>
/// </summary>
[HtmlTargetElement("currency")]
public class CurrencyTagHelper : TagHelper
{
    public decimal Amount { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "span";
        output.TagMode = TagMode.StartTagAndEndTag;

        var arabic = new CultureInfo("ar-SA");
        // Use Latin digits with Arabic grouping for a clean admin look.
        var formatted = Amount.ToString("N0", CultureInfo.GetCultureInfo("en-US"));

        output.Content.SetContent($"{formatted} ر.س");
    }
}

