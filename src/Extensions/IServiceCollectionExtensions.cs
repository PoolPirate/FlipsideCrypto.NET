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
        _ = services.AddHttpClient<JsonRPCClient>()
            .AddStandardResilienceHandler();
        _ = services.AddSingleton<JsonRPCClient>();
        _ = services.AddSingleton<IFlipsideClient, FlipsideClient>();

        return services;
    }

    public static IServiceCollection AddFlipsideCrypto(this IServiceCollection services, 
        Action<IServiceProvider, FlipsideOptionBuilder> configure)
    {
        _ = services.AddSingleton<HttpClient>();
        _ = services.AddSingleton<JsonRPCClient>();
        _ = services.AddSingleton<IFlipsideClient, FlipsideClient>();

        _ = services.AddSingleton(provider =>
        {
            var builder = new FlipsideOptionBuilder();
            configure.Invoke(provider, builder);
            return builder.Build();
        });

        return services;
    }
}
