namespace AuctionMVC.Contracts;

/// <summary>Mirrors Application.DTOs.BidDto.</summary>
public class BidDto
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>Mirrors Application.DTOs.CreateBidDto.</summary>
public class CreateBidDto
{
    public Guid AuctionId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
}

