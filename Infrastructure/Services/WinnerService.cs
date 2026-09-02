using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Services;

public class WinnerService : IWinnerService
{
    private readonly ApplicationDbContext _context;

    public WinnerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Winner>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Winners
            .Include(w => w.Auction)
            .Include(w => w.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<Winner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Winners
            .Include(w => w.Auction)
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<Winner> CreateAsync(Winner winner, CancellationToken cancellationToken = default)
    {
        winner.Id = Guid.NewGuid();
        _context.Winners.Add(winner);
        await _context.SaveChangesAsync(cancellationToken);
        return winner;
    }

    public async Task DeleteAsync(Winner winner, CancellationToken cancellationToken = default)
    {
        _context.Winners.Remove(winner);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

