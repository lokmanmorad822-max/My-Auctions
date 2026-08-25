using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionMVC.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Error(string? message = null)
    {
        ViewData["Message"] = message ?? "حدث خطأ غير متوقع. يرجى المحاولة لاحقاً.";
        Response.StatusCode = 500;
        return View();
    }

public IActionResult NotFoundPage()
    {
        Response.StatusCode = 404;
        return View("NotFound");
    }
}

