using Microsoft.Extensions.Options;
using SearchEngineService.Options;
using SearchEngineService.Providers;
using SearchEngineService.Constants;

namespace SearchEngineService.Providers.Behavior;

public class XmlProviderRateLimitHandler(IOptions<ProviderOptions> providerOptions)
    : ProviderRateLimitDelegatingHandler(
        ProviderConstants.XmlProvider,
        TimeSpan.FromSeconds(Math.Max(providerOptions.Value.XmlRateLimitSeconds, 0)));
