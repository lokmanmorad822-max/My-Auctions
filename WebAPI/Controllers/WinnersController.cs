using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WinnersController : ControllerBase
{
    private readonly IWinnerService _winnerService;

    public WinnersController(IWinnerService winnerService)
    {
        _winnerService = winnerService;
    }

    [HttpGet]
    public async Task<ActionResult<List<WinnerDto>>> GetAll(CancellationToken cancellationToken)
    {
        var winners = await _winnerService.GetAllAsync(cancellationToken);

        var winnerDtos = winners.Select(w => new WinnerDto
        {
            Id = w.Id,
            AuctionId = w.AuctionId,
            UserId = w.UserId,
            FinalPrice = w.FinalPrice
        }).ToList();

        return Ok(winnerDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WinnerDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var winner = await _winnerService.GetByIdAsync(id, cancellationToken);
        if (winner == null) return NotFound();

        return Ok(new WinnerDto
        {
            Id = winner.Id,
            AuctionId = winner.AuctionId,
            UserId = winner.UserId,
            FinalPrice = winner.FinalPrice
        });
    }

    [HttpPost]
    public async Task<ActionResult<WinnerDto>> Create(CreateWinnerDto createDto, CancellationToken cancellationToken)
    {
        var winner = new Winner
        {
            AuctionId = createDto.AuctionId,
            UserId = createDto.UserId,
            FinalPrice = createDto.FinalPrice
        };

        var created = await _winnerService.CreateAsync(winner, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new WinnerDto
        {
            Id = created.Id,
            AuctionId = created.AuctionId,
            UserId = created.UserId,
            FinalPrice = created.FinalPrice
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var winner = await _winnerService.GetByIdAsync(id, cancellationToken);
        if (winner == null) return NotFound();

        await _winnerService.DeleteAsync(winner, cancellationToken);
        return NoContent();
    }
}

