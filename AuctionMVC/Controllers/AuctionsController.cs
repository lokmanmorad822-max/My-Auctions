using AuctionMVC.Exceptions;
using AuctionMVC.Services;
using AuctionMVC.ViewModels.Auctions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionMVC.Controllers;

[Authorize]
public class AuctionsController : Controller
{
    private readonly IAuctionManagementService _auctionService;

    public AuctionsController(IAuctionManagementService auctionService)
    {
        _auctionService = auctionService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? status, string? search, CancellationToken ct)
    {
        ViewData["Title"] = "المزادات";
        var model = await _auctionService.GetIndexAsync(status, search, ct);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        ViewData["Title"] = "تفاصيل المزاد";
        var model = await _auctionService.GetDetailsAsync(id, ct);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        ViewData["Title"] = "إنشاء مزاد جديد";
        var model = await _auctionService.GetCreateModelAsync(ct);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AuctionFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            // Repopulate dropdowns on validation failure.
            var fresh = await _auctionService.GetCreateModelAsync(ct);
            model.AvailableProducts = fresh.AvailableProducts;
            model.AvailableUsers = fresh.AvailableUsers;
            return View(model);
        }

        try
        {
            var id = await _auctionService.CreateAsync(model, ct);
            TempData["SuccessMessage"] = "تم إنشاء المزاد بنجاح.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var fresh = await _auctionService.GetCreateModelAsync(ct);
            model.AvailableProducts = fresh.AvailableProducts;
            model.AvailableUsers = fresh.AvailableUsers;
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        ViewData["Title"] = "تعديل المزاد";
        var model = await _auctionService.GetEditModelAsync(id, ct);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AuctionFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var fresh = await _auctionService.GetEditModelAsync(id, ct);
            model.AvailableProducts = fresh.AvailableProducts;
            model.AvailableUsers = fresh.AvailableUsers;
            return View(model);
        }

        try
        {
            await _auctionService.UpdateAsync(id, model, ct);
            TempData["SuccessMessage"] = "تم حفظ تعديلات المزاد بنجاح.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var fresh = await _auctionService.GetEditModelAsync(id, ct);
            model.AvailableProducts = fresh.AvailableProducts;
            model.AvailableUsers = fresh.AvailableUsers;
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid id, CancellationToken ct)
    {
        await _auctionService.ApproveAsync(id, ct);
        TempData["SuccessMessage"] = "تمت الموافقة على المزاد بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid id, CancellationToken ct)
    {
        await _auctionService.RejectAsync(id, ct);
        TempData["SuccessMessage"] = "تم رفض المزاد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Stop(Guid id, CancellationToken ct)
    {
        await _auctionService.StopAsync(id, ct);
        TempData["SuccessMessage"] = "تم إيقاف المزاد نهائياً.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _auctionService.DeleteAsync(id, ct);
        TempData["SuccessMessage"] = "تم حذف المزاد.";
        return RedirectToAction(nameof(Index));
    }
}

