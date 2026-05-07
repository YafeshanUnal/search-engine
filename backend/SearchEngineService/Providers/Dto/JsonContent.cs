using System.Text.Json.Serialization;
using SearchEngineService.Constants;

namespace SearchEngineService.Providers.Dto;

public sealed class JsonContent
{
    [JsonPropertyName(JsonConstants.IdProperty)]
    public required string Id { get; init; }
    
    [JsonPropertyName(JsonConstants.TitleProperty)]
    public required string Title { get; init; }
    
    [JsonPropertyName(JsonConstants.TypeProperty)]
    public required string Type { get; init; }
    
    [JsonPropertyName(JsonConstants.MetricsProperty)]
    public required JsonMetrics Metrics { get; init; }
    
    [JsonPropertyName(JsonConstants.PublishedAtProperty)]
    public DateTime PublishedAtUtc { get; init; }
}
