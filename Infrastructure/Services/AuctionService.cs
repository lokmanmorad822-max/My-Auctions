using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Services;

public class AuctionService : IAuctionService
{
    private readonly ApplicationDbContext _context;

    public AuctionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Auction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Auctions
            .Include(a => a.Product)
            .Include(a => a.User)
            .Include(a => a.Bids)
            .Include(a => a.Winner)
            .ToListAsync(cancellationToken);
    }

    public async Task<Auction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Auctions
            .Include(a => a.Product)
            .Include(a => a.User)
            .Include(a => a.Bids)
            .Include(a => a.Winner)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Auction> CreateAuctionAsync(Auction auction, CancellationToken cancellationToken = default)
    {
        auction.Id = Guid.NewGuid();
        auction.Status = AuctionStatus.Pending;
        _context.Auctions.Add(auction);
        await _context.SaveChangesAsync(cancellationToken);
        return auction;
    }

    public async Task<Auction> UpdateAsync(Auction auction, CancellationToken cancellationToken = default)
    {
        _context.Auctions.Update(auction);
        await _context.SaveChangesAsync(cancellationToken);
        return auction;
    }

    public async Task DeleteAsync(Auction auction, CancellationToken cancellationToken = default)
    {
        _context.Auctions.Remove(auction);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Auction?> ApproveAuctionAsync(Guid auctionId, CancellationToken cancellationToken = default)
    {
        var auction = await GetByIdAsync(auctionId, cancellationToken);
        if (auction == null) return null;

        auction.Status = AuctionStatus.Active;
        return await UpdateAsync(auction, cancellationToken);
    }

    public async Task<Auction?> RejectAuctionAsync(Guid auctionId, CancellationToken cancellationToken = default)
    {
        var auction = await GetByIdAsync(auctionId, cancellationToken);
        if (auction == null) return null;

        auction.Status = AuctionStatus.Rejected;
        return await UpdateAsync(auction, cancellationToken);
    }

    public async Task<Auction?> StopAuctionAsync(Guid auctionId, CancellationToken cancellationToken = default)
    {
        var auction = await GetByIdAsync(auctionId, cancellationToken);
        if (auction == null) return null;

        auction.Status = AuctionStatus.Stopped;
        return await UpdateAsync(auction, cancellationToken);
    }
}

