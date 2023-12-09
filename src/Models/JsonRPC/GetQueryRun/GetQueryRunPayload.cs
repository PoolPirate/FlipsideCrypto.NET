using FlipsideCrypto.NET.JsonRPC.Models;
using FlipsideCrypto.NET.Models.ValueObjects;

namespace FlipsideCrypto.NET.Models.JsonRPC.GetQueryRun;
internal class GetQueryRunPayload : IJsonRPCPayload
{
    public static string Method => "getQueryRun";

    public required QueryRunId QueryRunId { get; init; }
}
