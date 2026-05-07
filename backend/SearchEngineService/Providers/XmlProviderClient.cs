using System.Xml.Linq;
using SearchEngineService.Models;
using SearchEngineService.Constants;

namespace SearchEngineService.Providers;

public class XmlProviderClient(HttpClient httpClient) : IProviderClient
{
    public string Name => ProviderConstants.XmlProvider;

    public async Task<IReadOnlyList<ProviderContentDto>> FetchAsync(CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(string.Empty, cancellationToken);
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        var doc = XDocument.Parse(payload);
        var items = doc.Root?.Element(XmlConstants.RootItems)?.Elements(XmlConstants.Item) ?? [];

        return items.Select(x =>
        {
            var kind = x.Element(XmlConstants.TypeElement)?.Value ?? ProviderConstants.DefaultType;
            var stats = x.Element(XmlConstants.StatsElement);
            return new ProviderContentDto
            {
                ExternalId = x.Element(XmlConstants.IdElement)?.Value ?? Guid.NewGuid().ToString("N"),
                Title = x.Element(XmlConstants.HeadlineElement)?.Value ?? ProviderConstants.Untitled,
                Type = kind.Equals(ProviderConstants.VideoType, StringComparison.OrdinalIgnoreCase) ? ContentType.Video : ContentType.Text,
                Views = ParseDouble(stats?.Element(XmlConstants.ViewsElement)?.Value),
                Likes = ParseDouble(stats?.Element(XmlConstants.LikesElement)?.Value),
                Reactions = ParseDouble(stats?.Element(XmlConstants.ReactionsElement)?.Value),
                ReadingTime = ParseDouble(stats?.Element(XmlConstants.ReadingTimeElement)?.Value),
                PublishedAtUtc = DateTime.TryParse(x.Element(XmlConstants.PublicationDateElement)?.Value, out var parsed)
                    ? parsed.ToUniversalTime()
                    : DateTime.UtcNow
            };
        }).ToList();
    }

    private static double ParseDouble(string? value) => double.TryParse(value, out var num) ? num : 0;
}
