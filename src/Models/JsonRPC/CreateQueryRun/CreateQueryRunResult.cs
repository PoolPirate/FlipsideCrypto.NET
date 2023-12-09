using FlipsideCrypto.NET.JsonRPC.Models;

namespace FlipsideCrypto.NET.Models.JsonRPC.CreateQueryRun;
internal class CreateQueryRunResult : IJsonRPCResult
{
    public required QueryRequest QueryRequest { get; init; }
    public required QueryRun QueryRun { get; init; }
    public required SqlStatement SqlStatement { get; init; }
}
