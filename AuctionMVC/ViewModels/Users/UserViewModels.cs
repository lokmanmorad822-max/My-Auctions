using System.ComponentModel.DataAnnotations;

namespace AuctionMVC.ViewModels.Users;

public class UserIndexViewModel
{
    public IReadOnlyList<UserListItemViewModel> Items { get; set; }
        = Array.Empty<UserListItemViewModel>();

    public string? Search { get; set; }
    public int TotalUsers { get; set; }

    /// <summary>Alias for TotalUsers, used in views.</summary>
    public int TotalCount => TotalUsers;
}

public class UserListItemViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int AuctionCount { get; set; }
    public int BidCount { get; set; }
    public int WinCount { get; set; }

    /// <summary>Alias for WinCount, used in views.</summary>
    public int WinnerCount => WinCount;

    /// <summary>First letters of the name for avatar display.</summary>
    public string Initials => string.IsNullOrWhiteSpace(Name)
        ? "?"
        : string.Join("", Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(w => w[0]));
}

public class UserFormViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "الاسم مطلوب.")]
    [MaxLength(100, ErrorMessage = "الاسم يجب ألا يتجاوز 100 حرف.")]
    [Display(Name = "الاسم الكامل")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
    [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة.")]
    [MaxLength(200, ErrorMessage = "البريد الإلكتروني يجب ألا يتجاوز 200 حرف.")]
    [Display(Name = "البريد الإلكتروني")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "رقم الجوال مطلوب.")]
    [Phone(ErrorMessage = "صيغة رقم الجوال غير صحيحة.")]
    [MaxLength(20, ErrorMessage = "رقم الجوال يجب ألا يتجاوز 20 رقماً.")]
    [Display(Name = "رقم الجوال")]
    public string Phone { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "كلمة المرور يجب ألا تقل عن 6 أحرف.")]
    [Display(Name = "كلمة المرور")]
    public string? Password { get; set; }

    public bool IsEdit => Id != Guid.Empty;

    /// <summary>Date of creation. Used in edit view display only.</summary>
    public DateTime CreatedAt { get; set; }
}

