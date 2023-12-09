using FlipsideCrypto.NET.JsonRPC.Models;

namespace FlipsideCrypto.NET.Models.JsonRPC.CreateQueryRun;
internal class CreateQueryRunPayload : IJsonRPCPayload
{
    public static string Method => "createQueryRun";

    public required int ResultTTLHours { get; init; }
    public required int MaxAgeMinutes { get; init; }
    public required string Sql { get; init; }

    public required IReadOnlyDictionary<string, string> Tags { get; init; }

    public required string DataSource { get; init; }
    public required string DataProvider { get; init; }
}
