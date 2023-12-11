using FlipsideCrypto.NET.JsonRPC.Models;
using Polly;
using System.Net.Http.Json;
using System.Text.Json;

namespace FlipsideCrypto.NET.JsonRPC.Services;
internal class JsonRPCClient
{
    private const string _jsonRPCVersion = "2.0";
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
        AllowTrailingCommas = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly HttpClient _httpClient;

    public JsonRPCClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void AddRequestHeader(string header, string value)
        => _httpClient.DefaultRequestHeaders.Add(header, value);

    public async Task<TResult> SendAsync<TPayload, TResult>(Uri uri, TPayload payload, CancellationToken cancellationToken = default)
        where TPayload : IJsonRPCPayload
        where TResult : IJsonRPCResult
    {
        var request = new JsonRPCRequest<TPayload>(_jsonRPCVersion, payload, 1);
        var httpResponse = await _httpClient.PostAsJsonAsync(uri, request, _jsonSerializerOptions, cancellationToken);

        _ = httpResponse.EnsureSuccessStatusCode();

        var response = await httpResponse.Content.ReadFromJsonAsync<JsonRPCResponse<TResult>>(cancellationToken)
            ?? throw new JsonException($"JsonRPC response could not be parsed to type {typeof(JsonRPCResponse<TResult>).FullName}");

        if(response.Id != request.Id)
        {
            throw new InvalidOperationException("JsonRPC response id differed from request id");
        }
        if(response.Error is not null)
        {
            throw new HttpRequestException($"JsonRPC responded with error: Code {response.Error.ErrorCode}, {response.Error.Message}");
        }
        //
        return response.Result
           ?? throw new JsonException($"JsonRPC response did not include a result");

    }
}
