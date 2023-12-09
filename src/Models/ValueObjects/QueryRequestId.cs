using FlipsideCrypto.NET.JsonConverters;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace FlipsideCrypto.NET.Models.ValueObjects;
[JsonConverter(typeof(ValueObjectConverter<QueryRequestId>))]
public readonly struct QueryRequestId : IValueObject<QueryRequestId>
{
    public readonly string Id;

    public QueryRequestId()
    {
        throw new ArgumentNullException(nameof(Id));
    }

    public QueryRequestId(string id)
    {
        Id = id;
    }

    static string IValueObject<QueryRequestId>.Serialize(QueryRequestId valueObject)
        => $"\"{valueObject.Id}\"";
    static QueryRequestId IValueObject<QueryRequestId>.Deserialize(string rawValue)
        => new QueryRequestId(rawValue[1..^1]);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is QueryRequestId other && other.Id.Equals(Id);

    public static bool operator ==(QueryRequestId left, QueryRequestId right)
        => left.Equals(right);

    public static bool operator !=(QueryRequestId left, QueryRequestId right)
        => !(left == right);

    public override int GetHashCode()
        => Id.GetHashCode();

    public override string ToString()
        => Id;
}
