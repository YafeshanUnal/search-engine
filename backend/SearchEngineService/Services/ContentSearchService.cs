using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SearchEngineService.Data;
using SearchEngineService.Models;
using SearchEngineService.Constants;
using SearchEngineService.Repositories;

namespace SearchEngineService.Services;

public class ContentSearchService(IContentRepository contentRepository, IMemoryCache cache)
{
    public async Task<PagedResult<ContentItem>> SearchAsync(SearchQuery query, CancellationToken cancellationToken)
    {
        var key = string.Format(CacheConstants.SearchKeyFormat, query.Keyword, query.Type, query.SortBy, query.Page, query.PageSize);
        if (cache.TryGetValue<PagedResult<ContentItem>>(key, out var cached) && cached is not null)
        {
            return cached;
        }

        // Try to get all contents from cache first
        if (cache.TryGetValue<IEnumerable<ContentItem>>(CacheConstants.AllContentsKey, out var allCached) && allCached is not null)
        {
            return SearchFromCachedData(allCached, query, key);
        }

        // If not in cache, get from database
        var allContents = await contentRepository.GetAllAsync(cancellationToken);
        
        // Cache all contents for future searches
        cache.Set(CacheConstants.AllContentsKey, allContents, TimeSpan.FromMinutes(CacheConstants.AllContentsCacheDuration));
        
        return SearchFromCachedData(allContents, query, key);
    }

    private PagedResult<ContentItem> SearchFromCachedData(IEnumerable<ContentItem> allContents, SearchQuery query, string cacheKey)
    {
        var dbQuery = allContents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim().ToLowerInvariant();
            dbQuery = dbQuery.Where(x => x.Title.ToLower().Contains(keyword));
        }

        if (query.Type.HasValue)
        {
            dbQuery = dbQuery.Where(x => x.Type == query.Type.Value);
        }

        dbQuery = query.SortBy.ToLowerInvariant() switch
        {
            SortConstants.Popularity => dbQuery.OrderByDescending(x => x.PopularityScore),
            SortConstants.Relevance => dbQuery.OrderByDescending(x => x.RelevanceScore),
            _ => dbQuery.OrderByDescending(x => x.FinalScore)
        };

        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var total = dbQuery.Count();
        var items = dbQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var result = new PagedResult<ContentItem>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };

        cache.Set(cacheKey, result, TimeSpan.FromMinutes(CacheConstants.SearchCacheDuration));
        return result;
    }
}
