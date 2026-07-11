using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class BidDto
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateBidDto
{
    [Required]
    public Guid AuctionId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
}

