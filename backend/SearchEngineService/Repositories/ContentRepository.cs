using Microsoft.EntityFrameworkCore;
using SearchEngineService.Data;
using SearchEngineService.Models;

namespace SearchEngineService.Repositories;

public class ContentRepository(AppDbContext dbContext) : Repository<ContentItem>(dbContext), IContentRepository
{
    public async Task<ContentItem?> GetByProviderAndExternalIdAsync(string provider, string externalId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.Provider == provider && x.ExternalId == externalId, cancellationToken);
    }

    public async Task<IEnumerable<ContentItem>> GetByProviderAsync(string provider, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Provider == provider)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ContentItem>> GetByTypeAsync(ContentType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Type == type)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByProviderAndExternalIdAsync(string provider, string externalId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(x => x.Provider == provider && x.ExternalId == externalId, cancellationToken);
    }
}
