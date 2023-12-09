using FlipsideCrypto.NET.Models.ValueObjects;

namespace FlipsideCrypto.NET.Models;
public class SqlStatement
{
    public required SqlStatementId Id { get; init; }
    public required string StatementHash { get; init; }
    public required string Sql { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset? UpdatedAt { get; init; }
}
