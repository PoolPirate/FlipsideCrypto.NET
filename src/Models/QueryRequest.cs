using FlipsideCrypto.NET.Models.ValueObjects;

namespace FlipsideCrypto.NET.Models;
public class QueryRequest
{
    public required QueryRequestId Id { get; init; }
    public required QueryRunId QueryRunId { get; init; }

    public required int MaxAgeMinutes { get; init; }
    public required int ResultTTLHours { get; init; }

    public required bool UserSkipCache { get; init; }
    public required bool TriggeredQueryRun { get; init; }
}
