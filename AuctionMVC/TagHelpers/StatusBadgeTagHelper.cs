using AuctionMVC.Contracts;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AuctionMVC.TagHelpers;

/// <summary>
/// Renders an auction status as an RTL Bootstrap pill matching the design:
/// <code><status-badge status="Active" /></code>
/// </summary>
[HtmlTargetElement("status-badge")]
public class StatusBadgeTagHelper : TagHelper
{
    public AuctionStatus Status { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "span";
        output.TagMode = TagMode.StartTagAndEndTag;

        var (css, label, icon) = Status switch
        {
            AuctionStatus.Pending => ("bg-soft-warning text-gold", "قيد المراجعة", "bi-hourglass-split"),
            AuctionStatus.Active => ("bg-soft-success", "نشط", "bi-lightning-charge-fill"),
            AuctionStatus.Finished => ("bg-soft-secondary", "منتهي", "bi-flag-fill"),
            AuctionStatus.Rejected => ("bg-soft-danger", "مرفوض", "bi-x-circle-fill"),
            AuctionStatus.Stopped => ("bg-soft-secondary", "متوقف", "bi-stop-circle-fill"),
            _ => ("bg-soft-secondary", "غير معروف", "bi-question-circle")
        };

        output.Attributes.SetAttribute("class", $"badge {css}");
        output.Content.SetHtmlContent($"<i class=\"bi {icon} me-1\"></i>{label}");
    }
}

