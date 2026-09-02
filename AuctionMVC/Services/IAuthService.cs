using System.Security.Claims;
using AuctionMVC.ViewModels.Account;

namespace AuctionMVC.Services;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(LoginViewModel model, CancellationToken ct = default);
    Task SignOutAsync();
}

/// <summary>Result of an authentication attempt.</summary>
public class LoginResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public ClaimsPrincipal? Principal { get; set; }
}

