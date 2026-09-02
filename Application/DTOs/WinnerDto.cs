using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class WinnerDto
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public Guid UserId { get; set; }
    public decimal FinalPrice { get; set; }
}

public class CreateWinnerDto
{
    [Required]
    public Guid AuctionId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal FinalPrice { get; set; }
}

