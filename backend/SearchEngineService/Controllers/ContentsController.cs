using Microsoft.AspNetCore.Mvc;
using SearchEngineService.Models;
using SearchEngineService.Services;
using SearchEngineService.Constants;

namespace SearchEngineService.Controllers;

/// <summary>
/// Content management and search API endpoints
/// </summary>
[ApiController]
[Route("api/contents")]
[Produces("application/json")]
public class ContentsController(ContentSearchService searchService, ContentIngestionService ingestionService) : ControllerBase
{
    /// <summary>
    /// Search contents with filters and pagination
    /// </summary>
    /// <param name="keyword">Search keyword in title</param>
    /// <param name="type">Content type filter (Video or Text)</param>
    /// <param name="sortBy">Sort by field (final, popularity, relevance)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated search results</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ContentItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ContentItem>>> Search(
        [FromQuery] string? keyword,
        [FromQuery] ContentType? type,
        [FromQuery] string sortBy = "final",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await searchService.SearchAsync(new SearchQuery
        {
            Keyword = keyword,
            Type = type,
            SortBy = sortBy,
            Page = page,
            PageSize = pageSize
        }, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Sync data from all configured providers
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sync result with imported count</returns>
    [HttpPost("sync")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> Sync(CancellationToken cancellationToken)
    {
        var count = await ingestionService.SyncAsync(cancellationToken);
        return Ok(new { message = "Sync tamamlandi", imported = count });
    }
}
