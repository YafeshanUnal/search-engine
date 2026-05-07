using System.Text.Json.Serialization;
using SearchEngineService.Constants;

namespace SearchEngineService.Providers.Dto;

public sealed class JsonMetrics
{
    [JsonPropertyName(JsonConstants.ViewsProperty)]
    public double Views { get; init; }
    
    [JsonPropertyName(JsonConstants.LikesProperty)]
    public double Likes { get; init; }
}
