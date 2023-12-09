using FlipsideCrypto.NET.JsonRPC.Models;

namespace FlipsideCrypto.NET.Models.JsonRPC.CancelQueryRun;
internal class CancelQueryRunResult : IJsonRPCResult
{
    public required QueryRun CancelledQueryRun { get; init; }
}
