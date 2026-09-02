using AuctionMVC.Contracts;

namespace AuctionMVC.ViewModels.Bids;

public class BidIndexViewModel
{
    public IReadOnlyList<BidListItemViewModel> Items { get; set; }
        = Array.Empty<BidListItemViewModel>();

    public string? Search { get; set; }
    public int TotalBids { get; set; }
    public decimal TotalAmount { get; set; }

    /// <summary>Alias for TotalBids, used in views.</summary>
    public int TotalCount => TotalBids;

    /// <summary>Optional auction ID filter, used in views.</summary>
    public string? AuctionId { get; set; }
}

public class BidListItemViewModel
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string BidderName { get; set; } = string.Empty;
    public string BidderEmail { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public AuctionStatus AuctionStatus { get; set; }

    /// <summary>Alias for ProductName, used in views.</summary>
    public string AuctionProductName => ProductName;

    /// <summary>First letters of the bidder name for avatar display.</summary>
    public string BidderInitials => string.IsNullOrWhiteSpace(BidderName)
        ? "?"
        : string.Join("", BidderName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(w => w[0]));
}

