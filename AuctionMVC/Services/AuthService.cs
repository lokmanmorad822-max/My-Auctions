using System.Security.Claims;
using System.Security.Principal;
using AuctionMVC.Contracts;
using AuctionMVC.Exceptions;
using AuctionMVC.Options;
using AuctionMVC.Services.Api;
using AuctionMVC.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace AuctionMVC.Services;

/// <summary>
/// Orchestrates authentication: tries the real backend login endpoint first and,
/// when the backend has no auth endpoints yet (404), falls back to the configured
/// local bridge so the dashboard remains fully usable.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IAuthApiClient _authApiClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthOptions _authOptions;

    public AuthService(
        IAuthApiClient authApiClient,
        IHttpContextAccessor httpContextAccessor,
        IOptions<AuthOptions> authOptions)
    {
        _authApiClient = authApiClient;
        _httpContextAccessor = httpContextAccessor;
        _authOptions = authOptions.Value;
    }

    public async Task<LoginResult> LoginAsync(LoginViewModel model, CancellationToken ct = default)
    {
        var result = new LoginResult();

        // 1) Try the real backend login endpoint.
        if (!_authOptions.LocalFallback.Enabled)
        {
            return await TryBackendLoginAsync(model, ct);
        }

        // 2) Backend login first; fall back to the local bridge only on 404 (endpoint missing).
        try
        {
            return await TryBackendLoginAsync(model, ct);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // TODO(BACKEND): remove local fallback once POST /api/auth/login exists.
            return await TryLocalFallbackAsync(model);
        }
        catch (ApiException ex)
        {
            // Credential error or connectivity issue surfaced to the user.
            result.Error = ex.Message;
            return result;
        }
    }

    private async Task<LoginResult> TryBackendLoginAsync(LoginViewModel model, CancellationToken ct)
    {
        try
        {
            var response = await _authApiClient.LoginAsync(new LoginRequest
            {
                Username = model.Username,
                Password = model.Password
            }, ct);

            if (string.IsNullOrWhiteSpace(response.Token))
            {
                return new LoginResult { Error = "لم يتم استلام رمز دخول صالح من الخادم." };
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, model.Username),
                new(ClaimTypes.Name, response.DisplayName ?? model.Username)
            };

            if (!string.IsNullOrWhiteSpace(response.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, response.Email));
            }

            claims.Add(new Claim("access_token", response.Token));

            foreach (var role in response.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return new LoginResult
            {
                Success = true,
                Principal = BuildPrincipal(claims)
            };
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return new LoginResult { Error = "اسم المستخدم أو كلمة المرور غير صحيحة." };
        }
    }

    private async Task<LoginResult> TryLocalFallbackAsync(LoginViewModel model)
    {
        var fallback = _authOptions.LocalFallback;

        if (string.Equals(model.Username?.Trim(), fallback.Username, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(model.Password, fallback.Password, StringComparison.Ordinal))
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, fallback.Username),
                new(ClaimTypes.Name, fallback.DisplayName),
                new(ClaimTypes.Role, "Admin"),
                new("auth_source", "local-fallback")
            };

            return new LoginResult
            {
                Success = true,
                Principal = BuildPrincipal(claims)
            };
        }

        return new LoginResult { Error = "اسم المستخدم أو كلمة المرور غير صحيحة." };
    }

    public async Task SignOutAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null) return;

        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    private static ClaimsPrincipal BuildPrincipal(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}

