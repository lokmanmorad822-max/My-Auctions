namespace AuctionMVC.Contracts;

/// <summary>Request contract for POST /api/auth/login.</summary>
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Expected response contract for POST /api/auth/login.
/// Field names are deliberately case-insensitive (System.Text.Json default) to
/// tolerate both camelCase and PascalCase backend responses.
/// </summary>
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public DateTime? ExpiresAt { get; set; }
}

