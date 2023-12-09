using FlipsideCrypto.NET.Configuration;
using FlipsideCrypto.NET.JsonRPC.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FlipsideCrypto.NET.Extensions;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddFlipsideCrypto(this IServiceCollection services,
        string apiKey, Action<FlipsideOptionBuilder>? configure = null)
    {
        var builder = new FlipsideOptionBuilder()
        {
            ApiKey = apiKey,
        };

        configure?.Invoke(builder);

        _ = services.AddSingleton(builder.Build());
        _ = services.AddSingleton<HttpClient>();
        _ = services.AddSingleton<JsonRPCClient>();
        _ = services.AddSingleton<IFlipsideClient, FlipsideClient>();

        return services;
    }
}
