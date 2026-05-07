using System.Text.Json;
using SearchEngineService.Models;
using SearchEngineService.Providers.Dto;
using SearchEngineService.Constants;

namespace SearchEngineService.Providers;

public class JsonProviderClient(HttpClient httpClient) : IProviderClient
{
    public string Name => ProviderConstants.JsonProvider;

    public async Task<IReadOnlyList<ProviderContentDto>> FetchAsync(CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(string.Empty, cancellationToken);
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        var apiResponse = JsonSerializer.Deserialize<JsonProviderResponse>(payload, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new JsonProviderResponse();
        return apiResponse.Contents.Select(x => new ProviderContentDto
        {
            ExternalId = x.Id,
            Title = x.Title,
            Type = x.Type.Equals(ProviderConstants.VideoType, StringComparison.OrdinalIgnoreCase) ? ContentType.Video : ContentType.Text,
            PublishedAtUtc = x.PublishedAtUtc,
            Views = x.Metrics.Views,
            Likes = x.Metrics.Likes,
            Reactions = 0,
            ReadingTime = 0
        }).ToList();
    }
}
