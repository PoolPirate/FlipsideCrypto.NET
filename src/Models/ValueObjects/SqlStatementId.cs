using FlipsideCrypto.NET.JsonConverters;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace FlipsideCrypto.NET.Models.ValueObjects;
[JsonConverter(typeof(ValueObjectConverter<SqlStatementId>))]
public readonly struct SqlStatementId : IValueObject<SqlStatementId>
{
    public readonly string Id;

    public SqlStatementId()
    {
        throw new ArgumentNullException(nameof(Id));
    }

    public SqlStatementId(string id)
    {
        Id = id;
    }

    static string IValueObject<SqlStatementId>.Serialize(SqlStatementId valueObject)
        => $"\"{valueObject.Id}\"";
    static SqlStatementId IValueObject<SqlStatementId>.Deserialize(string rawValue)
        => new SqlStatementId(rawValue[1..^1]);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is SqlStatementId other && other.Id.Equals(Id);

    public static bool operator ==(SqlStatementId left, SqlStatementId right)
        => left.Equals(right);

    public static bool operator !=(SqlStatementId left, SqlStatementId right)
        => !(left == right);

    public override int GetHashCode()
        => Id.GetHashCode();

    public override string ToString()
        => Id;
}
