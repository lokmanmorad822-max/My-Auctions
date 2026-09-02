namespace Domain.Entities;

public class Auction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public decimal StartPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public AuctionStatus Status { get; set; } = AuctionStatus.Pending;

    // Navigation properties
    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
    public Winner? Winner { get; set; }
}

