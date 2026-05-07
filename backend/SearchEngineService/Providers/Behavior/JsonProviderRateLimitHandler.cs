using Microsoft.Extensions.Options;
using SearchEngineService.Options;
using SearchEngineService.Providers;
using SearchEngineService.Constants;

namespace SearchEngineService.Providers.Behavior;

public class JsonProviderRateLimitHandler(IOptions<ProviderOptions> providerOptions)
    : ProviderRateLimitDelegatingHandler(
        ProviderConstants.JsonProvider,
        TimeSpan.FromSeconds(Math.Max(providerOptions.Value.JsonRateLimitSeconds, 0)));
