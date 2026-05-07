using SearchEngineService.Models;

namespace SearchEngineService.Repositories;

public interface IContentRepository : IRepository<ContentItem>
{
    Task<ContentItem?> GetByProviderAndExternalIdAsync(string provider, string externalId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContentItem>> GetByProviderAsync(string provider, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContentItem>> GetByTypeAsync(ContentType type, CancellationToken cancellationToken = default);
    Task<bool> ExistsByProviderAndExternalIdAsync(string provider, string externalId, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
