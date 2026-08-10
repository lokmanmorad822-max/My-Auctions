using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AuctionMVC.Exceptions;
using AuctionMVC.Options;
using Microsoft.Extensions.Options;

namespace AuctionMVC.Services.Api;

/// <summary>
/// Base class for all typed API clients.
/// - Centralizes the base address & timeout from <see cref="ApiOptions"/>.
/// - Serializes with System.Text.Json (camelCase, enums as strings).
/// - Attaches the JWT bearer token from the current user's claims.
/// - Normalizes HTTP failures into <see cref="ApiException"/>.
/// </summary>
public abstract class ApiClientBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    protected ApiClientBase(HttpClient httpClient, IOptions<ApiOptions> apiOptions, IHttpContextAccessor httpContextAccessor)
    {
        HttpClient = httpClient;
        ApiOptions = apiOptions.Value;
        HttpContextAccessor = httpContextAccessor;
    }

    protected HttpClient HttpClient { get; }
    protected ApiOptions ApiOptions { get; }
    protected IHttpContextAccessor HttpContextAccessor { get; }

    /// <summary>
    /// The base URL for this client (applied per-request so a single factory
    /// client can be reused even if the base URL changes at runtime).
    /// </summary>
    protected virtual string BaseUrl => ApiOptions.BaseUrl.TrimEnd('/');

    /// <summary>Attach bearer token when the current user is authenticated.</summary>
    protected void AttachAuthHeader()
    {
        var token = HttpContextAccessor.HttpContext?.User?
            .FindFirst("access_token")?.Value;

        if (!string.IsNullOrWhiteSpace(token))
        {
            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            HttpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    protected async Task<T> GetAsync<T>(string path, CancellationToken ct = default)
    {
        AttachAuthHeader();
        var response = await HttpClient.GetAsync($"{BaseUrl}{path}", ct);
        return await ReadAsync<T>(response, ct);
    }

    protected async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string path,
        object? payload = null,
        CancellationToken ct = default)
    {
        AttachAuthHeader();

        using var request = new HttpRequestMessage(method, $"{BaseUrl}{path}");
        if (payload is not null)
        {
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return await HttpClient.SendAsync(request, ct);
    }

    protected async Task<T> SendAsync<T>(HttpMethod method, string path, object? payload = null, CancellationToken ct = default)
    {
        using var response = await SendAsync(method, path, payload, ct);
        return await ReadAsync<T>(response, ct);
    }

    protected async Task<HttpResponseMessage> DeleteAsync(string path, CancellationToken ct = default)
    {
        AttachAuthHeader();
        return await HttpClient.DeleteAsync($"{BaseUrl}{path}", ct);
    }

    private static async Task<T> ReadAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        var content = await response.Content.ReadAsStringAsync(ct);

        if (response.IsSuccessStatusCode)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return default!;
            }

            return JsonSerializer.Deserialize<T>(content, JsonOptions) ?? default!;
        }

        throw await ApiException.FromResponseAsync(response, content);
    }
}

