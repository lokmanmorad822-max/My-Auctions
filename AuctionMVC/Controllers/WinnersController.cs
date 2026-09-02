using AuctionMVC.Exceptions;
using AuctionMVC.Services;
using AuctionMVC.ViewModels.Winners;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionMVC.Controllers;

[Authorize]
public class WinnersController : Controller
{
    private readonly IWinnerManagementService _winnerService;

    public WinnersController(IWinnerManagementService winnerService)
    {
        _winnerService = winnerService;
    }

[HttpGet]
    public async Task<IActionResult> Index(string? search, CancellationToken ct)
    {
        ViewData["Title"] = "الفائزون";
        var model = await _winnerService.GetIndexAsync(search, ct);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _winnerService.DeleteAsync(id, ct);
        TempData["SuccessMessage"] = "تم حذف سجل الفائز.";
        return RedirectToAction(nameof(Index));
    }
}

