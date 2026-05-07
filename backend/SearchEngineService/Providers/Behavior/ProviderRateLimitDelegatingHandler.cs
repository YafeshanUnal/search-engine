using System.Collections.Concurrent;

namespace SearchEngineService.Providers.Behavior;

public class ProviderRateLimitDelegatingHandler(string providerKey, TimeSpan minInterval) : DelegatingHandler
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> Locks = new();
    private static readonly ConcurrentDictionary<string, DateTime> LastCallUtc = new();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var limiter = Locks.GetOrAdd(providerKey, _ => new SemaphoreSlim(1, 1));
        await limiter.WaitAsync(cancellationToken);
        try
        {
            var now = DateTime.UtcNow;
            if (LastCallUtc.TryGetValue(providerKey, out var lastCall))
            {
                var elapsed = now - lastCall;
                if (elapsed < minInterval)
                {
                    await Task.Delay(minInterval - elapsed, cancellationToken);
                }
            }

            LastCallUtc[providerKey] = DateTime.UtcNow;
        }
        finally
        {
            limiter.Release();
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
