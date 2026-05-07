namespace SearchEngineService.Constants;

public static class CacheConstants
{
    public const string SearchKeyFormat = "search:{0}:{1}:{2}:{3}:{4}";
    public const string AllContentsKey = "contents:all";
    public const string ProviderDataKey = "provider:data:{0}";
    
    // Cache durations in minutes
    public const int SearchCacheDuration = 5;
    public const int AllContentsCacheDuration = 15;
    public const int ProviderDataCacheDuration = 10;
}
