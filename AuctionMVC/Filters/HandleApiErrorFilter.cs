using AuctionMVC.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuctionMVC.Filters;

/// <summary>
/// Global MVC filter that catches <see cref="ApiException"/> (and generic
/// exceptions) thrown by services during an HTTP request and redirects to a
/// friendly error page, preserving the failure message via TempData.
/// </summary>
public class HandleApiErrorFilter : IAsyncExceptionFilter
{
    private readonly ILogger<HandleApiErrorFilter> _logger;

    public HandleApiErrorFilter(ILogger<HandleApiErrorFilter> logger)
    {
        _logger = logger;
    }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        var isAjax = context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                     context.HttpContext.Request.Headers["HX-Request"] == "true";

        switch (context.Exception)
        {
            case ApiException apiEx:
                _logger.LogWarning(apiEx, "API error during {Path}", context.HttpContext.Request.Path);
                return HandleApiAsync(context, apiEx, isAjax);

            default:
                _logger.LogError(context.Exception, "Unhandled exception during {Path}", context.HttpContext.Request.Path);
                return HandleGenericAsync(context, isAjax);
        }
    }

    private static Task HandleApiAsync(ExceptionContext context, ApiException ex, bool isAjax)
    {
        if (isAjax)
        {
            context.Result = new JsonResult(new
            {
                success = false,
                message = ex.Message,
                errors = ex.Errors
            });
            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }

        context.HttpContext.Items["ApiError"] = ex.Message;

        // For validation errors, return to the referrer with a TempData message.
        context.Result = new RedirectToActionResult("Error", "Home", new
        {
            message = ex.Message
        });
        context.ExceptionHandled = true;
        return Task.CompletedTask;
    }

    private static Task HandleGenericAsync(ExceptionContext context, bool isAjax)
    {
        if (isAjax)
        {
            context.Result = new JsonResult(new
            {
                success = false,
                message = "حدث خطأ غير متوقع. يرجى المحاولة لاحقاً."
            });
            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }

        context.Result = new RedirectToActionResult("Error", "Home", new
        {
            message = "حدث خطأ غير متوقع. يرجى المحاولة لاحقاً."
        });
        context.ExceptionHandled = true;
        return Task.CompletedTask;
    }
}

