using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuctionsController : ControllerBase
{
    private readonly IAuctionService _auctionService;

    public AuctionsController(IAuctionService auctionService)
    {
        _auctionService = auctionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAll(CancellationToken cancellationToken)
    {
        var auctions = await _auctionService.GetAllAsync(cancellationToken);

        var auctionDtos = auctions.Select(a => new AuctionDto
        {
            Id = a.Id,
            UserId = a.UserId,
            ProductId = a.ProductId,
            StartPrice = a.StartPrice,
            CurrentPrice = a.CurrentPrice,
            StartDate = a.StartDate,
            EndDate = a.EndDate,
            Status = a.Status
        }).ToList();

        return Ok(auctionDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var auction = await _auctionService.GetByIdAsync(id, cancellationToken);
        if (auction == null) return NotFound();

        return Ok(new AuctionDto
        {
            Id = auction.Id,
            UserId = auction.UserId,
            ProductId = auction.ProductId,
            StartPrice = auction.StartPrice,
            CurrentPrice = auction.CurrentPrice,
            StartDate = auction.StartDate,
            EndDate = auction.EndDate,
            Status = auction.Status
        });
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> Create(CreateAuctionDto createDto, CancellationToken cancellationToken)
    {
        var auction = new Auction
        {
            UserId = createDto.UserId,
            ProductId = createDto.ProductId,
            StartPrice = createDto.StartPrice,
            CurrentPrice = createDto.CurrentPrice,
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
            Status = createDto.Status
        };

        var created = await _auctionService.CreateAuctionAsync(auction, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new AuctionDto
        {
            Id = created.Id,
            UserId = created.UserId,
            ProductId = created.ProductId,
            StartPrice = created.StartPrice,
            CurrentPrice = created.CurrentPrice,
            StartDate = created.StartDate,
            EndDate = created.EndDate,
            Status = created.Status
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AuctionDto>> Update(Guid id, UpdateAuctionDto updateDto, CancellationToken cancellationToken)
    {
        var auction = await _auctionService.GetByIdAsync(id, cancellationToken);
        if (auction == null) return NotFound();

        auction.StartPrice = updateDto.StartPrice;
        auction.CurrentPrice = updateDto.CurrentPrice;
        auction.StartDate = updateDto.StartDate;
        auction.EndDate = updateDto.EndDate;
        auction.Status = updateDto.Status;

        var updated = await _auctionService.UpdateAsync(auction, cancellationToken);

        return Ok(new AuctionDto
        {
            Id = updated.Id,
            UserId = updated.UserId,
            ProductId = updated.ProductId,
            StartPrice = updated.StartPrice,
            CurrentPrice = updated.CurrentPrice,
            StartDate = updated.StartDate,
            EndDate = updated.EndDate,
            Status = updated.Status
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var auction = await _auctionService.GetByIdAsync(id, cancellationToken);
        if (auction == null) return NotFound();

        await _auctionService.DeleteAsync(auction, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id}/approve")]
    public async Task<ActionResult<AuctionDto>> Approve(Guid id, CancellationToken cancellationToken)
    {
        var auction = await _auctionService.ApproveAuctionAsync(id, cancellationToken);
        if (auction == null) return NotFound();

        return Ok(new AuctionDto
        {
            Id = auction.Id,
            UserId = auction.UserId,
            ProductId = auction.ProductId,
            StartPrice = auction.StartPrice,
            CurrentPrice = auction.CurrentPrice,
            StartDate = auction.StartDate,
            EndDate = auction.EndDate,
            Status = auction.Status
        });
    }

    [HttpPost("{id}/reject")]
    public async Task<ActionResult<AuctionDto>> Reject(Guid id, CancellationToken cancellationToken)
    {
        var auction = await _auctionService.RejectAuctionAsync(id, cancellationToken);
        if (auction == null) return NotFound();

        return Ok(new AuctionDto
        {
            Id = auction.Id,
            UserId = auction.UserId,
            ProductId = auction.ProductId,
            StartPrice = auction.StartPrice,
            CurrentPrice = auction.CurrentPrice,
            StartDate = auction.StartDate,
            EndDate = auction.EndDate,
            Status = auction.Status
        });
    }

    [HttpPost("{id}/stop")]
    public async Task<ActionResult<AuctionDto>> Stop(Guid id, CancellationToken cancellationToken)
    {
        var auction = await _auctionService.StopAuctionAsync(id, cancellationToken);
        if (auction == null) return NotFound();

        return Ok(new AuctionDto
        {
            Id = auction.Id,
            UserId = auction.UserId,
            ProductId = auction.ProductId,
            StartPrice = auction.StartPrice,
            CurrentPrice = auction.CurrentPrice,
            StartDate = auction.StartDate,
            EndDate = auction.EndDate,
            Status = auction.Status
        });
    }
}

