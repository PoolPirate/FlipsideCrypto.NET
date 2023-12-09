using FlipsideCrypto.NET.JsonRPC.Models;

namespace FlipsideCrypto.NET.Models.JsonRPC.GetQueryRun;
internal class GetQueryRunResult : IJsonRPCResult
{
    public required QueryRun QueryRun { get; init; }
}
