using Microsoft.Extensions.Options;
using SearchEngineService.Models;
using SearchEngineService.Options;

namespace SearchEngineService.Services;

public class ScoreCalculator(IOptions<ScoringOptions> scoringOptions)
{
    private readonly ScoringOptions _settings = scoringOptions.Value;

    public double CalculateFinalScore(ContentItem item)
    {
        var baseScore = CalculateBaseScore(item);
        var typeFactor = item.Type == ContentType.Video ? _settings.VideoTypeFactor : _settings.TextTypeFactor;
        var recency = CalculateRecencyScore(item.PublishedAtUtc);
        var interaction = CalculateInteractionScore(item);
        return (baseScore * typeFactor) + recency + interaction;
    }

    public double CalculatePopularityScore(ContentItem item)
        => item.Type == ContentType.Video
            ? (item.Views / _settings.VideoBaseViewsDivisor) + item.Likes
            : item.Reactions + (item.ReadingTime * _settings.TextPopularityReadingTimeWeight);

    private double CalculateBaseScore(ContentItem item)
        => item.Type == ContentType.Video
            ? (item.Views / _settings.VideoBaseViewsDivisor) + (item.Likes / _settings.VideoBaseLikesDivisor)
            : item.ReadingTime + (item.Reactions / _settings.TextBaseReactionsDivisor);

    private double CalculateRecencyScore(DateTime publishedAtUtc)
    {
        var age = DateTime.UtcNow - publishedAtUtc;
        if (age <= TimeSpan.FromDays(_settings.RecencyWeekDays)) return _settings.RecencyWeekScore;
        if (age <= TimeSpan.FromDays(_settings.RecencyMonthDays)) return _settings.RecencyMonthScore;
        if (age <= TimeSpan.FromDays(_settings.RecencyQuarterDays)) return _settings.RecencyQuarterScore;
        return _settings.RecencyOldScore;
    }

    private double CalculateInteractionScore(ContentItem item)
    {
        if (item.Type == ContentType.Video)
        {
            if (item.Views <= 0) return 0;
            return (item.Likes / item.Views) * _settings.VideoInteractionMultiplier;
        }

        if (item.ReadingTime <= 0) return 0;
        return (item.Reactions / item.ReadingTime) * _settings.TextInteractionMultiplier;
    }
}
