namespace SearchEngineService.Models;

public class SearchQuery
{
    public string? Keyword { get; init; }
    public ContentType? Type { get; init; }
    public string SortBy { get; init; } = "final";
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
