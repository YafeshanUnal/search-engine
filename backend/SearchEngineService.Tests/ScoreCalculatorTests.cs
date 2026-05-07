using Microsoft.Extensions.Options;
using SearchEngineService.Models;
using SearchEngineService.Options;
using SearchEngineService.Services;

namespace SearchEngineService.Tests;

public class ScoreCalculatorTests
{
    [Fact]
    public void CalculateFinalScore_ForVideo_UsesConfiguredFormula()
    {
        var calculator = BuildCalculator();
        var item = new ContentItem
        {
            ExternalId = "v1",
            Provider = "provider",
            Title = "Video test",
            Type = ContentType.Video,
            Views = 15000,
            Likes = 1200,
            Reactions = 0,
            ReadingTime = 0,
            PublishedAtUtc = DateTime.UtcNow.AddDays(-5)
        };

        var result = calculator.CalculateFinalScore(item);
        var expected = ((15000d / 1000d + 1200d / 100d) * 1.5d) + 5d + ((1200d / 15000d) * 10d);

        Assert.Equal(expected, result, 6);
    }

    [Fact]
    public void CalculateFinalScore_ForText_UsesConfiguredFormula()
    {
        var calculator = BuildCalculator();
        var item = new ContentItem
        {
            ExternalId = "t1",
            Provider = "provider",
            Title = "Text test",
            Type = ContentType.Text,
            Views = 0,
            Likes = 0,
            Reactions = 200,
            ReadingTime = 10,
            PublishedAtUtc = DateTime.UtcNow.AddDays(-20)
        };

        var result = calculator.CalculateFinalScore(item);
        var expected = ((10d + 200d / 50d) * 1.0d) + 3d + ((200d / 10d) * 5d);

        Assert.Equal(expected, result, 6);
    }

    [Fact]
    public void CalculateFinalScore_WhenOlderThanQuarter_UsesOldContentScore()
    {
        var calculator = BuildCalculator();
        var item = new ContentItem
        {
            ExternalId = "old",
            Provider = "provider",
            Title = "Old text",
            Type = ContentType.Text,
            Reactions = 50,
            ReadingTime = 5,
            PublishedAtUtc = DateTime.UtcNow.AddDays(-120)
        };

        var result = calculator.CalculateFinalScore(item);
        var expected = ((5d + 50d / 50d) * 1.0d) + 0d + ((50d / 5d) * 5d);

        Assert.Equal(expected, result, 6);
    }

    private static ScoreCalculator BuildCalculator()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new ScoringOptions());
        return new ScoreCalculator(options);
    }
}
