namespace SearchEngineService.Constants;

public static class ConfigurationConstants
{
    public const string ScoringSection = "Scoring";
    public const string ProvidersSection = "Providers";
    public const string DefaultConnection = "Host=localhost;Port=5432;Database=searchdb;Username=postgres;Password=postgres";
    
    public const string DashboardPolicy = "dashboard";
    public const string JsonUrlKey = "Providers:JsonUrl";
    public const string XmlUrlKey = "Providers:XmlUrl";
}
