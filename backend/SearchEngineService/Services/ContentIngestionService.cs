using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SearchEngineService.Models;
using SearchEngineService.Constants;
using SearchEngineService.Providers;
using SearchEngineService.Repositories;

namespace SearchEngineService.Services;

public class ContentIngestionService(
    IEnumerable<IProviderClient> providers,
    IContentRepository contentRepository,
    IMemoryCache cache,
    ScoreCalculator scoreCalculator,
    ILogger<ContentIngestionService> logger)
{
    public async Task<int> SyncAsync(CancellationToken cancellationToken)
    {
        var merged = new List<ContentItem>();

        foreach (var provider in providers)
        {
            try
            {
                // Check cache first
                var cacheKey = string.Format(CacheConstants.ProviderDataKey, provider.Name);
                if (cache.TryGetValue(cacheKey, out List<ProviderContentDto>? cachedPayload))
                {
                    logger.LogInformation("Using cached data for provider: {Provider}", provider.Name);
                    merged.AddRange(cachedPayload.Select(dto => ToContent(provider.Name, dto, scoreCalculator)));
                    continue;
                }

                // Fetch from provider if not in cache
                logger.LogInformation("Fetching data from provider: {Provider}", provider.Name);
                var payload = await provider.FetchAsync(cancellationToken);
                
                // Cache the provider data
                cache.Set(cacheKey, payload?.ToList() ?? [], TimeSpan.FromMinutes(CacheConstants.ProviderDataCacheDuration));
                
                merged.AddRange(payload.Select(dto => ToContent(provider.Name, dto, scoreCalculator)));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Provider sync failed: {Provider}", provider.Name);
            }
        }

        // Clear all contents cache since we're syncing fresh data
        cache.Remove(CacheConstants.AllContentsKey);

        foreach (var item in merged)
        {
            var existing = await contentRepository.GetByProviderAndExternalIdAsync(
                item.Provider, 
                item.ExternalId, 
                cancellationToken);

            if (existing is null)
            {
                await contentRepository.AddAsync(item, cancellationToken);
                continue;
            }

            existing.Title = item.Title;
            existing.Type = item.Type;
            existing.Views = item.Views;
            existing.Likes = item.Likes;
            existing.Reactions = item.Reactions;
            existing.ReadingTime = item.ReadingTime;
            existing.PublishedAtUtc = item.PublishedAtUtc;
            existing.RelevanceScore = item.RelevanceScore;
            existing.PopularityScore = item.PopularityScore;
            existing.FinalScore = item.FinalScore;
            existing.LastSyncedAtUtc = DateTime.UtcNow;
            
            await contentRepository.UpdateAsync(existing, cancellationToken);
        }

        await contentRepository.SaveChangesAsync(cancellationToken);
        
        // Cache all contents from database
        await CacheAllContentsAsync(cancellationToken);
        
        return merged.Count;
    }

    private async Task CacheAllContentsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var allContents = await contentRepository.GetAllAsync(cancellationToken);
            cache.Set(CacheConstants.AllContentsKey, allContents, TimeSpan.FromMinutes(CacheConstants.AllContentsCacheDuration));
            logger.LogInformation("Cached {Count} contents from database", allContents.Count());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error caching all contents");
        }
    }

    private static ContentItem ToContent(string providerName, ProviderContentDto dto, ScoreCalculator scoreCalculator)
    {
        var now = DateTime.UtcNow;
        var content = new ContentItem
        {
            ExternalId = dto.ExternalId,
            Provider = providerName,
            Title = dto.Title,
            Type = dto.Type,
            Views = dto.Views,
            Likes = dto.Likes,
            Reactions = dto.Reactions,
            ReadingTime = dto.ReadingTime,
            PublishedAtUtc = dto.PublishedAtUtc,
            CreatedAtUtc = now,
            LastSyncedAtUtc = now
        };

        content.FinalScore = scoreCalculator.CalculateFinalScore(content);
        content.PopularityScore = scoreCalculator.CalculatePopularityScore(content);
        content.RelevanceScore = content.FinalScore;
        return content;
    }
}
