using System.Text.Json.Serialization;

namespace FlipsideCrypto.NET.JsonRPC.Models;
internal class JsonRPCResponse<TResult>
    where TResult : IJsonRPCResult
{
    [JsonPropertyName("jsonrpc")]
    public required string JsonRpc { get; init; }

    public TResult? Result { get; init; }
    public JsonRPCError? Error { get; init; }

    public required int Id { get; init; }
}
