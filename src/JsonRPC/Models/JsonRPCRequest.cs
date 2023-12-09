using System.Text.Json.Serialization;

namespace FlipsideCrypto.NET.JsonRPC.Models;
internal class JsonRPCRequest<TPayload>
    where TPayload : IJsonRPCPayload
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; }
    public string Method { get; }
    public TPayload[] Params { get; }
    public int Id { get; }

    public JsonRPCRequest(string version, TPayload payload, int id)
    {
        JsonRpc = version;
        Method = TPayload.Method;
        Params = [payload];
        Id = id;
    }
}
