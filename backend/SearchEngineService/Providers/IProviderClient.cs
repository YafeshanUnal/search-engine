namespace SearchEngineService.Providers;

public interface IProviderClient
{
    string Name { get; }
    Task<IReadOnlyList<ProviderContentDto>> FetchAsync(CancellationToken cancellationToken);
}
