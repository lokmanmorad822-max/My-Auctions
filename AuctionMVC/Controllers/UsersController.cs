using AuctionMVC.Exceptions;
using AuctionMVC.Services;
using AuctionMVC.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionMVC.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly IUserManagementService _userService;

    public UsersController(IUserManagementService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, CancellationToken ct)
    {
        ViewData["Title"] = "المستخدمون";
        var model = await _userService.GetIndexAsync(search, ct);
        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "إنشاء مستخدم";
        return View(new UserFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var id = await _userService.CreateAsync(model, ct);
            TempData["SuccessMessage"] = "تم إنشاء المستخدم بنجاح.";
            return RedirectToAction(nameof(Edit), new { id });
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        ViewData["Title"] = "تعديل المستخدم";
        var model = await _userService.GetEditModelAsync(id, ct);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UserFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _userService.UpdateAsync(id, model, ct);
            TempData["SuccessMessage"] = "تم حفظ تعديلات المستخدم.";
            return RedirectToAction(nameof(Edit), new { id });
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _userService.DeleteAsync(id, ct);
        TempData["SuccessMessage"] = "تم حذف المستخدم.";
        return RedirectToAction(nameof(Index));
    }
}

