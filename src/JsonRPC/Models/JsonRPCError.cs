using System.Text.Json.Serialization;

namespace FlipsideCrypto.NET.JsonRPC.Models;
public class JsonRPCError
{
    [JsonPropertyName("code")]
    public required int ErrorCode { get; init; }
    public required string Message { get; init; }
}
