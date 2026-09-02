namespace Domain.Entities;

public class Winner
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public Guid UserId { get; set; }
    public decimal FinalPrice { get; set; }

    // Navigation properties
    public Auction Auction { get; set; } = null!;
    public User User { get; set; } = null!;
}

