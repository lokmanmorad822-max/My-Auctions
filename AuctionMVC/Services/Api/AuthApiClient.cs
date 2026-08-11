using AuctionMVC.Contracts;
using AuctionMVC.Options;
using Microsoft.Extensions.Options;

namespace AuctionMVC.Services.Api;

public interface IAuthApiClient
{
    /// <summary>
    /// Calls POST {LoginEndpoint}. Throws ApiException when the backend has no
    /// auth endpoint (404) so callers can fall back to the local bridge.
    /// </summary>
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
}

/// <summary>
/// Typed client for the (future) <c>/api/auth</c> endpoints.
/// The backend does not implement these yet — see TODO markers.
/// </summary>
public class AuthApiClient : ApiClientBase, IAuthApiClient
{
    public AuthApiClient(
        HttpClient httpClient,
        IOptions<ApiOptions> apiOptions,
        IHttpContextAccessor httpContextAccessor,
        IOptions<AuthOptions> authOptions)
        : base(httpClient, apiOptions, httpContextAccessor)
    {
        AuthOptions = authOptions.Value;
    }

    protected AuthOptions AuthOptions { get; }

    /// <summary>
    /// TODO(BACKEND): Implement POST /api/auth/login in AuctionAPI.
    /// Currently the endpoint does not exist and returns 404, which the
    /// AuthService handles by falling back to the configured local bridge.
    /// </summary>
    public Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        => SendAsync<LoginResponse>(HttpMethod.Post, AuthOptions.LoginEndpoint, request, ct);
}

