using Domain.Entities;

namespace Application.Interfaces;

public interface IBidService
{
    Task<List<Bid>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Bid?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Bid> PlaceBidAsync(Bid bid, CancellationToken cancellationToken = default);
    Task<bool> ValidateBidAsync(Guid auctionId, decimal amount, CancellationToken cancellationToken = default);
}

