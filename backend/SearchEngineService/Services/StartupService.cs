using Microsoft.Extensions.Caching.Memory;
using SearchEngineService.Constants;
using SearchEngineService.Repositories;
using SearchEngineService.Services;

namespace SearchEngineService.Services;

/// <summary>
/// Service to handle startup tasks like auto-sync
/// </summary>
public class StartupService(
    IContentRepository contentRepository,
    ContentIngestionService ingestionService,
    IMemoryCache cache,
    ILogger<StartupService> logger)
{
    /// <summary>
    /// Perform startup tasks including auto-sync
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task PerformStartupTasksAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Starting application startup tasks...");

            // Check if we have any content in database
            var existingContentCount = await contentRepository.CountAsync(cancellationToken);
            
            if (existingContentCount == 0)
            {
                logger.LogInformation("No existing content found. Performing initial sync...");
                var importedCount = await ingestionService.SyncAsync(cancellationToken);
                logger.LogInformation("Initial sync completed. Imported {Count} items.", importedCount);
            }
            else
            {
                logger.LogInformation("Found {Count} existing items. Skipping initial sync.", existingContentCount);
                
                // Cache existing contents for faster first search
                var existingContents = await contentRepository.GetAllAsync(cancellationToken);
                cache.Set(CacheConstants.AllContentsKey, existingContents, TimeSpan.FromMinutes(CacheConstants.AllContentsCacheDuration));
                logger.LogInformation("Cached {Count} existing contents for faster access.", existingContents.Count());
            }

            logger.LogInformation("Startup tasks completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during startup tasks");
            // Don't throw - allow application to start even if sync fails
        }
    }
}
