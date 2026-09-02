namespace AuctionMVC.Contracts;

/// <summary>Mirrors Application.DTOs.AuctionDto.</summary>
public class AuctionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public decimal StartPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public AuctionStatus Status { get; set; }
}

/// <summary>Mirrors Application.DTOs.CreateAuctionDto.</summary>
public class CreateAuctionDto
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public decimal StartPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public AuctionStatus Status { get; set; } = AuctionStatus.Pending;
}

/// <summary>Mirrors Application.DTOs.UpdateAuctionDto.</summary>
public class UpdateAuctionDto
{
    public decimal StartPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public AuctionStatus Status { get; set; }
}

