using FlipsideCrypto.NET.Configuration;
using FlipsideCrypto.NET.JsonRPC.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly.Retry;

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

        _ = services.AddHttpClient<JsonRPCClient>()
            .AddStandardResilienceHandler(options => options.Retry.ShouldHandle = (RetryPredicateArguments<HttpResponseMessage> args) => new ValueTask<bool>(HttpClientResiliencePredicates.IsTransient(args.Outcome) || args.Outcome.Result?.StatusCode == System.Net.HttpStatusCode.BadGateway || args.Outcome.Result?.StatusCode == System.Net.HttpStatusCode.GatewayTimeout));

        _ = services.AddSingleton(builder.Build());
        _ = services.AddSingleton<JsonRPCClient>();
        _ = services.AddSingleton<IFlipsideClient, FlipsideClient>();

        return services;
    }

    public static IServiceCollection AddFlipsideCrypto(this IServiceCollection services,
        Action<IServiceProvider, FlipsideOptionBuilder> configure)
    {
        _ = services.AddHttpClient<JsonRPCClient>()
            .AddStandardResilienceHandler(options => options.Retry.ShouldHandle = (RetryPredicateArguments<HttpResponseMessage> args) => new ValueTask<bool>(HttpClientResiliencePredicates.IsTransient(args.Outcome)
                || args.Outcome.Result?.StatusCode == System.Net.HttpStatusCode.BadGateway
                || args.Outcome.Result?.StatusCode == System.Net.HttpStatusCode.GatewayTimeout
                || (args.Outcome.Exception is HttpRequestException ex && (
                    ex.StatusCode == System.Net.HttpStatusCode.BadGateway || ex.StatusCode == System.Net.HttpStatusCode.GatewayTimeout))
              ));

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
