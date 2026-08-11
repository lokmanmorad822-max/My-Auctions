namespace AuctionMVC.Options;

/// <summary>
/// Authentication-related configuration.
/// Bound from the "Auth" section of appsettings.json.
/// </summary>
public class AuthOptions
{
    public const string SectionName = "Auth";

    /// <summary>Backend login endpoint (expected POST /api/auth/login).</summary>
    public string LoginEndpoint { get; set; } = "/api/auth/login";

    /// <summary>Backend token refresh endpoint.</summary>
    public string RefreshEndpoint { get; set; } = "/api/auth/refresh";

    /// <summary>
    /// Temporary bridge so the dashboard can be used before the backend auth
    /// endpoints exist. When the real endpoint is available, set Enabled=false.
    /// </summary>
    public LocalFallbackOptions LocalFallback { get; set; } = new();
}

public class LocalFallbackOptions
{
    public bool Enabled { get; set; } = true;
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = "Admin@123";
    public string DisplayName { get; set; } = "مدير النظام";
}

