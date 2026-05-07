using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SearchEngineService.Data;
using SearchEngineService.Models;
using SearchEngineService.Services;

namespace SearchEngineService.Tests;

public class ContentSearchServiceTests
{
    [Fact]
    public async Task SearchAsync_FiltersByKeywordAndType_AndPaginates()
    {
        await using var db = BuildDbContext();
        await SeedAsync(db);
        var service = new ContentSearchService(db, BuildCache());

        var result = await service.SearchAsync(new SearchQuery
        {
            Keyword = "go",
            Type = ContentType.Video,
            SortBy = "final",
            Page = 1,
            PageSize = 2
        }, CancellationToken.None);

        Assert.Equal(2, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, item =>
        {
            Assert.Equal(ContentType.Video, item.Type);
            Assert.Contains("go", item.Title, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    public async Task SearchAsync_SortsByPopularityDescending()
    {
        await using var db = BuildDbContext();
        await SeedAsync(db);
        var service = new ContentSearchService(db, BuildCache());

        var result = await service.SearchAsync(new SearchQuery
        {
            SortBy = "popularity",
            Page = 1,
            PageSize = 10
        }, CancellationToken.None);

        Assert.True(result.Items.Count >= 2);
        Assert.True(result.Items[0].PopularityScore >= result.Items[1].PopularityScore);
    }

    [Fact]
    public async Task SearchAsync_SortsByRelevanceDescending()
    {
        await using var db = BuildDbContext();
        await SeedAsync(db);
        var service = new ContentSearchService(db, BuildCache());

        var result = await service.SearchAsync(new SearchQuery
        {
            SortBy = "relevance",
            Page = 1,
            PageSize = 10
        }, CancellationToken.None);

        Assert.True(result.Items.Count >= 2);
        Assert.True(result.Items[0].RelevanceScore >= result.Items[1].RelevanceScore);
    }

    private static AppDbContext BuildDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        return new AppDbContext(options);
    }

    private static IMemoryCache BuildCache()
        => new MemoryCache(new MemoryCacheOptions());

    private static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Contents.AnyAsync())
        {
            return;
        }

        db.Contents.AddRange([
            new ContentItem
            {
                ExternalId = "1",
                Provider = "json",
                Title = "Go Programming Tutorial",
                Type = ContentType.Video,
                Views = 15000,
                Likes = 1200,
                PopularityScore = 1215,
                RelevanceScore = 70,
                FinalScore = 70,
                PublishedAtUtc = DateTime.UtcNow.AddDays(-5)
            },
            new ContentItem
            {
                ExternalId = "2",
                Provider = "json",
                Title = "Advanced Go Concurrency Patterns",
                Type = ContentType.Video,
                Views = 25000,
                Likes = 2100,
                PopularityScore = 2125,
                RelevanceScore = 85,
                FinalScore = 85,
                PublishedAtUtc = DateTime.UtcNow.AddDays(-10)
            },
            new ContentItem
            {
                ExternalId = "3",
                Provider = "xml",
                Title = "Clean Architecture in Go",
                Type = ContentType.Text,
                Reactions = 450,
                ReadingTime = 8,
                PopularityScore = 466,
                RelevanceScore = 40,
                FinalScore = 40,
                PublishedAtUtc = DateTime.UtcNow.AddDays(-20)
            }
        ]);

        await db.SaveChangesAsync();
    }
}
