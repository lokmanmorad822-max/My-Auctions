using Domain.Entities;

namespace Application.Interfaces;

public interface IAuctionService
{
    Task<List<Auction>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Auction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Auction> CreateAuctionAsync(Auction auction, CancellationToken cancellationToken = default);
    Task<Auction> UpdateAsync(Auction auction, CancellationToken cancellationToken = default);
    Task DeleteAsync(Auction auction, CancellationToken cancellationToken = default);
    Task<Auction?> ApproveAuctionAsync(Guid auctionId, CancellationToken cancellationToken = default);
    Task<Auction?> RejectAuctionAsync(Guid auctionId, CancellationToken cancellationToken = default);
    Task<Auction?> StopAuctionAsync(Guid auctionId, CancellationToken cancellationToken = default);
}

