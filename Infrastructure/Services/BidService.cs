using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Services;

public class BidService : IBidService
{
    private readonly ApplicationDbContext _context;

    public BidService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Bid>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Bids
            .Include(b => b.Auction)
            .Include(b => b.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<Bid?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Bids
            .Include(b => b.Auction)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<Bid> PlaceBidAsync(Bid bid, CancellationToken cancellationToken = default)
    {
        bid.Id = Guid.NewGuid();
        _context.Bids.Add(bid);
        await _context.SaveChangesAsync(cancellationToken);
        return bid;
    }

    public async Task<bool> ValidateBidAsync(Guid auctionId, decimal amount, CancellationToken cancellationToken = default)
    {
        var auction = await _context.Auctions
            .FirstOrDefaultAsync(a => a.Id == auctionId, cancellationToken);
        if (auction == null) return false;
        if (auction.Status != AuctionStatus.Active) return false;
        if (amount <= auction.CurrentPrice) return false;
        if (DateTime.UtcNow < auction.StartDate || DateTime.UtcNow > auction.EndDate) return false;

        return true;
    }
}

