namespace AuctionMVC.Contracts;

/// <summary>
/// Mirrors Domain.Entities.AuctionStatus from the AuctionAPI.
/// </summary>
public enum AuctionStatus
{
    Pending = 0,
    Active = 1,
    Finished = 2,
    Rejected = 3,
    Stopped = 4
}

