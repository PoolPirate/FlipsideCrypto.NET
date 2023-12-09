using FlipsideCrypto.NET.JsonConverters;
using System.Text.Json.Serialization;

namespace FlipsideCrypto.NET.Models;
[JsonConverter(typeof(QueryStateConverter))]
public enum QueryState
{
    Ready,
    Running,
    StreamingResults,
    Success,
    Failed,
    Cancelled
}
