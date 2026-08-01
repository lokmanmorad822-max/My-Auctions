using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BidsController : ControllerBase
{
    private readonly IBidService _bidService;
    private readonly IAuctionService _auctionService;

    public BidsController(IBidService bidService, IAuctionService auctionService)
    {
        _bidService = bidService;
        _auctionService = auctionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<BidDto>>> GetAll(CancellationToken cancellationToken)
    {
        var bids = await _bidService.GetAllAsync(cancellationToken);

        var bidDtos = bids.Select(b => new BidDto
        {
            Id = b.Id,
            AuctionId = b.AuctionId,
            UserId = b.UserId,
            Amount = b.Amount,
            CreatedAt = b.CreatedAt
        }).ToList();

        return Ok(bidDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BidDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var bid = await _bidService.GetByIdAsync(id, cancellationToken);
        if (bid == null) return NotFound();

        return Ok(new BidDto
        {
            Id = bid.Id,
            AuctionId = bid.AuctionId,
            UserId = bid.UserId,
            Amount = bid.Amount,
            CreatedAt = bid.CreatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<BidDto>> Create(CreateBidDto createDto, CancellationToken cancellationToken)
    {
        var isValid = await _bidService.ValidateBidAsync(createDto.AuctionId, createDto.Amount, cancellationToken);
        if (!isValid)
        {
            return BadRequest(new { error = "Bid is invalid. Check auction status, current price, and auction dates." });
        }

        var bid = new Bid
        {
            AuctionId = createDto.AuctionId,
            UserId = createDto.UserId,
            Amount = createDto.Amount
        };

        var created = await _bidService.PlaceBidAsync(bid, cancellationToken);

        // Update current price of the auction
        var auction = await _auctionService.GetByIdAsync(createDto.AuctionId, cancellationToken);
        if (auction != null)
        {
            auction.CurrentPrice = createDto.Amount;
            await _auctionService.UpdateAsync(auction, cancellationToken);
        }

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new BidDto
        {
            Id = created.Id,
            AuctionId = created.AuctionId,
            UserId = created.UserId,
            Amount = created.Amount,
            CreatedAt = created.CreatedAt
        });
    }
}

