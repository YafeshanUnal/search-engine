using SearchEngineService.Models;

namespace SearchEngineService.Providers;

public class ProviderContentDto
{
    public required string ExternalId { get; init; }
    public required string Title { get; init; }
    public required ContentType Type { get; init; }
    public required DateTime PublishedAtUtc { get; init; }
    public double Views { get; init; }
    public double Likes { get; init; }
    public double Reactions { get; init; }
    public double ReadingTime { get; init; }
}
