using Domain.Entities;

namespace Application.Interfaces;

public interface IWinnerService
{
    Task<List<Winner>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Winner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Winner> CreateAsync(Winner winner, CancellationToken cancellationToken = default);
    Task DeleteAsync(Winner winner, CancellationToken cancellationToken = default);
}

