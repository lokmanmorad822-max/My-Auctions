namespace AuctionMVC.Options;

/// <summary>
/// Strongly-typed configuration for the AuctionAPI connection.
/// Bound from the "Api" section of appsettings.json.
/// </summary>
public class ApiOptions
{
    public const string SectionName = "Api";

    /// <summary>Base URL of the AuctionAPI (e.g. http://localhost:5051).</summary>
    public string BaseUrl { get; set; } = "http://localhost:5051";

    /// <summary>HTTP client timeout in seconds.</summary>
    public int TimeoutSeconds { get; set; } = 30;
}

