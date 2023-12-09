using FlipsideCrypto.NET.JsonRPC.Models;
using System.Text.Json.Nodes;

namespace FlipsideCrypto.NET.Models.JsonRPC.GetQueryRunResults;
internal class GetQueryRunCSVResultsResult : IJsonRPCResult
{
    public required string[] ColumnNames { get; init; }
    public required string[] ColumnTypes { get; init; }

    public required JsonNode[][] Rows { get; init; }

    public required QueryPagination Page { get; init; }
    public required QueryRun OriginalQueryRun { get; init; }
}
