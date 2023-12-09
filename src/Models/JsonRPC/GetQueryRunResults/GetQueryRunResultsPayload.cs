using FlipsideCrypto.NET.JsonRPC.Models;
using FlipsideCrypto.NET.Models.ValueObjects;

namespace FlipsideCrypto.NET.Models.JsonRPC.GetQueryRunResults;
internal class GetQueryRunResultsPayload : IJsonRPCPayload
{
    public static string Method => "getQueryRunResults";

    public required QueryRunId QueryRunId { get; init; }
    public required string Format { get; init; }
    public required QueryResultsPage Page { get; init; }
}
