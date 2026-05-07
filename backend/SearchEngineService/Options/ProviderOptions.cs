namespace SearchEngineService.Options;

public class ProviderOptions
{
    public string? JsonUrl { get; set; }
    public string? XmlUrl { get; set; }
    public int JsonRateLimitSeconds { get; set; } = 2;
    public int XmlRateLimitSeconds { get; set; } = 2;
}
