using FlipsideCrypto.NET.Models.ValueObjects;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlipsideCrypto.NET.JsonConverters;
internal class ValueObjectConverter<TValueObject> : JsonConverter<TValueObject>
    where TValueObject : IValueObject<TValueObject>
{
    public override TValueObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        return TValueObject.Deserialize(doc.RootElement.GetRawText() ?? throw new JsonException("Could not read QueryRunId"));
    }

    public override void Write(Utf8JsonWriter writer, TValueObject value, JsonSerializerOptions options)
    {
        string serialized = TValueObject.Serialize(value);
        writer.WriteRawValue(serialized);
    }
}
