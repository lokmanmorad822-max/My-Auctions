using AuctionMVC.Services;
using AuctionMVC.ViewModels.Bids;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionMVC.Controllers;

[Authorize]
public class BidsController : Controller
{
    private readonly IBidManagementService _bidService;

    public BidsController(IBidManagementService bidService)
    {
        _bidService = bidService;
    }

[HttpGet]
    public async Task<IActionResult> Index(string? auctionId, string? search, CancellationToken ct)
    {
        ViewData["Title"] = "المزايدات";
        var model = await _bidService.GetIndexAsync(auctionId, search, ct);
        return View(model);
    }
}

