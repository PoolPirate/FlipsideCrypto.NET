using FlipsideCrypto.NET.JsonConverters;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace FlipsideCrypto.NET.Models.ValueObjects;
[JsonConverter(typeof(ValueObjectConverter<QueryRunId>))]
public readonly struct QueryRunId : IValueObject<QueryRunId>
{
    public readonly string Id;

    public QueryRunId()
    {
        throw new ArgumentNullException(nameof(Id));
    }

    public QueryRunId(string id)
    {
        Id = id;
    }

    static string IValueObject<QueryRunId>.Serialize(QueryRunId valueObject)
        => $"\"{valueObject.Id}\"";
    static QueryRunId IValueObject<QueryRunId>.Deserialize(string rawValue)
        => new QueryRunId(rawValue[1..^1]);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is QueryRunId other && other.Id.Equals(Id);

    public static bool operator ==(QueryRunId left, QueryRunId right)
        => left.Equals(right);

    public static bool operator !=(QueryRunId left, QueryRunId right)
        => !(left == right);

    public override int GetHashCode()
        => Id.GetHashCode();

    public override string ToString()
        => Id;
}
