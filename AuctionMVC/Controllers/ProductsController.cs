using AuctionMVC.Exceptions;
using AuctionMVC.Services;
using AuctionMVC.ViewModels.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionMVC.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly IProductManagementService _productService;

    public ProductsController(IProductManagementService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, string? category, CancellationToken ct)
    {
        ViewData["Title"] = "المنتجات";
        var model = await _productService.GetIndexAsync(search, category, ct);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        ViewData["Title"] = "إنشاء منتج";
        var model = await _productService.GetCreateModelAsync(ct);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var fresh = await _productService.GetCreateModelAsync(ct);
            model.AvailableCategories = fresh.AvailableCategories;
            return View(model);
        }

        try
        {
            var id = await _productService.CreateAsync(model, ct);
            TempData["SuccessMessage"] = "تم إنشاء المنتج بنجاح.";
            return RedirectToAction(nameof(Edit), new { id });
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var fresh = await _productService.GetCreateModelAsync(ct);
            model.AvailableCategories = fresh.AvailableCategories;
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        ViewData["Title"] = "تعديل المنتج";
        var model = await _productService.GetEditModelAsync(id, ct);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ProductFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var fresh = await _productService.GetEditModelAsync(id, ct);
            model.AvailableCategories = fresh.AvailableCategories;
            return View(model);
        }

        try
        {
            await _productService.UpdateAsync(id, model, ct);
            TempData["SuccessMessage"] = "تم حفظ تعديلات المنتج.";
            return RedirectToAction(nameof(Edit), new { id });
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var fresh = await _productService.GetEditModelAsync(id, ct);
            model.AvailableCategories = fresh.AvailableCategories;
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _productService.DeleteAsync(id, ct);
        TempData["SuccessMessage"] = "تم حذف المنتج.";
        return RedirectToAction(nameof(Index));
    }
}

