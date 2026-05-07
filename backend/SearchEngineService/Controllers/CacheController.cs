using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SearchEngineService.Constants;

namespace SearchEngineService.Controllers;

/// <summary>
/// Cache management and debugging endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CacheController(IMemoryCache cache) : ControllerBase
{
    /// <summary>
    /// Get all cache keys and values for debugging
    /// </summary>
    [HttpGet]
    public IActionResult GetCacheInfo()
    {
        var cacheInfo = new
        {
            AllContentsKey = GetCacheValue(CacheConstants.AllContentsKey),
            AllContentsCount = GetCacheItemCount(CacheConstants.AllContentsKey),
            Timestamp = DateTime.UtcNow
        };

        return Ok(cacheInfo);
    }

    /// <summary>
    /// Clear all cache
    /// </summary>
    [HttpDelete]
    public IActionResult ClearCache()
    {
        cache.Remove(CacheConstants.AllContentsKey);
        
        // Clear provider caches
        var providers = new[] { "provider-json", "provider-xml" };
        foreach (var provider in providers)
        {
            cache.Remove(string.Format(CacheConstants.ProviderDataKey, provider));
        }

        return Ok(new { message = "Cache cleared successfully" });
    }

    private object? GetCacheValue(string key)
    {
        if (cache.TryGetValue(key, out var value))
        {
            return value switch
            {
                IEnumerable<object> items => new { count = items.Count(), sample = items.Take(3).Select(x => new { title = GetProperty(x, "Title") }) },
                _ => value
            };
        }
        return null;
    }

    private int GetCacheItemCount(string key)
    {
        if (cache.TryGetValue(key, out var value) && value is IEnumerable<object> items)
        {
            return items.Count();
        }
        return 0;
    }

    private string? GetProperty(object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName)?.GetValue(obj)?.ToString();
    }
}
