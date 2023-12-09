using FlipsideCrypto.NET.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlipsideCrypto.NET.JsonConverters;
internal class QueryStateConverter : JsonConverter<QueryState>
{
    public override QueryState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        return value switch
        {
            "QUERY_STATE_READY" => QueryState.Ready,
            "QUERY_STATE_RUNNING" => QueryState.Running,
            "QUERY_STATE_STREAMING_RESULTS" => QueryState.StreamingResults,
            "QUERY_STATE_SUCCESS" => QueryState.Success,
            "QUERY_STATE_FAILED" => QueryState.Failed,
            "QUERY_STATE_CANCELED" => QueryState.Cancelled,
            _ => throw new JsonException($"Could not convert {value} to {nameof(QueryState)}")
        };
    }

    public override void Write(Utf8JsonWriter writer, QueryState value, JsonSerializerOptions options)
        => writer.WriteStringValue(value switch
        {
            QueryState.Ready => "QUERY_STATE_READY",
            QueryState.Running => "QUERY_STATE_RUNNING",
            QueryState.StreamingResults => "QUERY_STATE_STREAMING_RESULTS",
            QueryState.Success => "QUERY_STATE_SUCCESS",
            QueryState.Failed => "QUERY_STATE_FAILED",
            QueryState.Cancelled => "QUERY_STATE_CANCELED",
            _ => throw new JsonException($"Could not convert {value} to json")
        });
}
