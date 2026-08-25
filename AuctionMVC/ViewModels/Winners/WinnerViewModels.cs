using System.ComponentModel.DataAnnotations;
using AuctionMVC.Contracts;

namespace AuctionMVC.ViewModels.Winners;

public class WinnerIndexViewModel
{
    public IReadOnlyList<WinnerListItemViewModel> Items { get; set; }
        = Array.Empty<WinnerListItemViewModel>();

    public string? Search { get; set; }
    public int TotalWinners { get; set; }
    public decimal TotalWinnings { get; set; }

    /// <summary>Alias for TotalWinners, used in views.</summary>
    public int TotalCount => TotalWinners;
}

public class WinnerListItemViewModel
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string WinnerName { get; set; } = string.Empty;
    public string WinnerEmail { get; set; } = string.Empty;
    public decimal FinalPrice { get; set; }
    public AuctionStatus AuctionStatus { get; set; }
    public DateTime? EndedAt { get; set; }

    /// <summary>Alias for ProductName, used in views.</summary>
    public string AuctionProductName => ProductName;

    /// <summary>First letters of the winner name for avatar display.</summary>
    public string WinnerInitials => string.IsNullOrWhiteSpace(WinnerName)
        ? "?"
        : string.Join("", WinnerName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(w => w[0]));
}

public class CreateWinnerViewModel
{
    [Required(ErrorMessage = "المزاد مطلوب.")]
    [Display(Name = "المزاد")]
    public Guid AuctionId { get; set; }

    [Required(ErrorMessage = "الفائز مطلوب.")]
    [Display(Name = "المستخدم الفائز")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "السعر النهائي مطلوب.")]
    [Range(0, double.MaxValue, ErrorMessage = "السعر النهائي لا يمكن أن يكون سالباً.")]
    [Display(Name = "السعر النهائي (ر.س)")]
    public decimal FinalPrice { get; set; }

    public IReadOnlyList<AuctionDto> AvailableAuctions { get; set; }
        = Array.Empty<AuctionDto>();

    public IReadOnlyList<UserDto> AvailableUsers { get; set; }
        = Array.Empty<UserDto>();
}

