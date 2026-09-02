using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Application.DTOs;

public class AuctionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public decimal StartPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public AuctionStatus Status { get; set; }
}

public class CreateAuctionDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal StartPrice { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal CurrentPrice { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public AuctionStatus Status { get; set; } = AuctionStatus.Pending;
}

public class UpdateAuctionDto
{
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal StartPrice { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal CurrentPrice { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public AuctionStatus Status { get; set; }
}

