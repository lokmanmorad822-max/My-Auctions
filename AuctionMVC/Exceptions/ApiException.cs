using System.Net;
using System.Text.Json;

namespace AuctionMVC.Exceptions;

/// <summary>
/// Represents a failed call to the AuctionAPI. Carries the HTTP status code and
/// a human-readable (Arabic) message, plus any model-state errors returned by the API.
/// </summary>
public class ApiException : Exception
{
    public ApiException(HttpStatusCode statusCode, string message, IReadOnlyDictionary<string, string[]>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    public HttpStatusCode StatusCode { get; }
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public bool IsValidationError => StatusCode == HttpStatusCode.BadRequest && Errors.Count > 0;

    /// <summary>
    /// Builds an <see cref="ApiException"/> from an HTTP response, attempting to
    /// parse both the API's simple {"error": "..."} envelope and ASP.NET's
    /// validation envelope {"errors": {field: [...]}}.
    /// </summary>
    public static async Task<ApiException> FromResponseAsync(HttpResponseMessage response, string content)
    {
        var status = response.StatusCode;

        // Try standard validation envelope (ASP.NET model state)
        if (status == HttpStatusCode.BadRequest)
        {
            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                if (root.TryGetProperty("errors", out var errorsElement) && errorsElement.ValueKind == JsonValueKind.Object)
                {
                    var errors = new Dictionary<string, string[]>();
                    foreach (var prop in errorsElement.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.Array)
                        {
                            errors[prop.Name] = prop.Value
                                .EnumerateArray()
                                .Where(x => x.ValueKind == JsonValueKind.String)
                                .Select(x => x.GetString() ?? string.Empty)
                                .ToArray();
                        }
                    }

                    if (errors.Count > 0)
                    {
                        return new ApiException(status, "البيانات المدخلة غير صحيحة. يرجى مراجعة الحقول.", errors);
                    }
                }
            }
            catch (JsonException)
            {
                // fall through to generic parsing
            }
        }

        var message = "تعذر الاتصال بالخادم. يرجى المحاولة لاحقاً.";

        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.TryGetProperty("error", out var errorEl) && errorEl.ValueKind == JsonValueKind.String)
            {
                var raw = errorEl.GetString();
                if (!string.IsNullOrWhiteSpace(raw))
                {
                    message = raw;
                }
            }
            else if (root.TryGetProperty("title", out var titleEl) && titleEl.ValueKind == JsonValueKind.String)
            {
                message = titleEl.GetString() ?? message;
            }
        }
        catch (JsonException)
        {
            if (!string.IsNullOrWhiteSpace(content) && content.Length < 500)
            {
                message = content;
            }
        }

        // Map a few well-known codes to Arabic messages.
        message = status switch
        {
            HttpStatusCode.NotFound => "العنصر المطلوب غير موجود.",
            HttpStatusCode.Unauthorized => "انتهت الجلسة أو أن صلاحياتك غير كافية. يرجى تسجيل الدخول مرة أخرى.",
            HttpStatusCode.Forbidden => "ليس لديك صلاحية لتنفيذ هذا الإجراء.",
            _ => message
        };

        return new ApiException(status, message);
    }
}

