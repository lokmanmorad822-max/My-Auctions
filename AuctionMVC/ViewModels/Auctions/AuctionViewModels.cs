using System.ComponentModel.DataAnnotations;
using AuctionMVC.Contracts;

namespace AuctionMVC.ViewModels.Auctions;

public class AuctionIndexViewModel
{
    public IReadOnlyList<AuctionListItemViewModel> Items { get; set; }
        = Array.Empty<AuctionListItemViewModel>();

    public AuctionStatusCountsViewModel Counts { get; set; } = new();

    public string CurrentStatus { get; set; } = "all";
    public string? Search { get; set; }
}

public class AuctionStatusCountsViewModel
{
    public int All { get; set; }
    public int Pending { get; set; }
    public int Active { get; set; }
    public int Finished { get; set; }
    public int Stopped { get; set; }
    public int Rejected { get; set; }
}

public class AuctionListItemViewModel
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public decimal StartPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public DateTime EndDate { get; set; }
    public int BidCount { get; set; }
    public AuctionStatus Status { get; set; }
}

public class AuctionDetailsViewModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Images { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public decimal StartPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public AuctionStatus Status { get; set; }
    public int BidCount { get; set; }
    public IReadOnlyList<AuctionBidViewModel> Bids { get; set; }
        = Array.Empty<AuctionBidViewModel>();

    public decimal? WinnerFinalPrice { get; set; }
    public string? WinnerUserName { get; set; }
}

public class AuctionBidViewModel
{
    public Guid Id { get; set; }
    public string BidderName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AuctionFormViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "المالك مطلوب.")]
    [Display(Name = "مالك المزاد")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "المنتج مطلوب.")]
    [Display(Name = "المنتج")]
    public Guid ProductId { get; set; }

    [Required(ErrorMessage = "سعر البداية مطلوب.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "سعر البداية يجب أن يكون أكبر من صفر.")]
    [Display(Name = "سعر البداية (ر.س)")]
    public decimal StartPrice { get; set; }

    [Required(ErrorMessage = "السعر الحالي مطلوب.")]
    [Range(0, double.MaxValue, ErrorMessage = "السعر الحالي لا يمكن أن يكون سالباً.")]
    [Display(Name = "السعر الحالي (ر.س)")]
    public decimal CurrentPrice { get; set; }

    [Required(ErrorMessage = "تاريخ البدء مطلوب.")]
    [Display(Name = "تاريخ ووقت البدء")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "تاريخ الانتهاء مطلوب.")]
    [Display(Name = "تاريخ ووقت الانتهاء")]
    public DateTime EndDate { get; set; }

    [Display(Name = "الحالة")]
    public AuctionStatus Status { get; set; }

    public IReadOnlyList<ProductDto> AvailableProducts { get; set; }
        = Array.Empty<ProductDto>();

    public IReadOnlyList<UserDto> AvailableUsers { get; set; }
        = Array.Empty<UserDto>();
}

