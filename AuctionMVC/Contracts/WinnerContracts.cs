namespace AuctionMVC.Contracts;

/// <summary>Mirrors Application.DTOs.WinnerDto.</summary>
public class WinnerDto
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public Guid UserId { get; set; }
    public decimal FinalPrice { get; set; }
}

/// <summary>Mirrors Application.DTOs.CreateWinnerDto.</summary>
public class CreateWinnerDto
{
    public Guid AuctionId { get; set; }
    public Guid UserId { get; set; }
    public decimal FinalPrice { get; set; }
}

