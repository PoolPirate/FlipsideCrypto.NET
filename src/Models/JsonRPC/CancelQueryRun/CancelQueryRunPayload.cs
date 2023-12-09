using FlipsideCrypto.NET.JsonRPC.Models;
using FlipsideCrypto.NET.Models.ValueObjects;

namespace FlipsideCrypto.NET.Models.JsonRPC.CancelQueryRun;
internal class CancelQueryRunPayload : IJsonRPCPayload
{
    public static string Method => "cancelQueryRun";

    public required QueryRunId QueryRunId { get; init; }
}
