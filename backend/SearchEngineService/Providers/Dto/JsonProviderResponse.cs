using System.Text.Json.Serialization;

namespace SearchEngineService.Providers.Dto;

public sealed class JsonProviderResponse
{
    public List<JsonContent> Contents { get; init; } = [];
}
