using FlipsideCrypto.NET.Models.ValueObjects;

namespace FlipsideCrypto.NET.Models;
public class QueryRun
{
    public required QueryRunId Id { get; init; }
    public required QueryState State { get; init; }

    public required int? RowCount { get; init; }
    public required int? TotalSize { get; init; }

    public required DateTimeOffset? StartedAt { get; init; }
    public required DateTimeOffset? QueryRunningEndedAt { get; init; }
    public required DateTimeOffset? QueryStreamingEndedAt { get; init; }
    public required DateTimeOffset? EndedAt { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset? UpdatedAt { get; init; }
    public required DateTimeOffset? ArchivedAt { get; init; }
}
