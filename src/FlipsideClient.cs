using FlipsideCrypto.NET.Configuration;
using FlipsideCrypto.NET.JsonRPC.Services;
using FlipsideCrypto.NET.Models;
using FlipsideCrypto.NET.Models.JsonRPC.CancelQueryRun;
using FlipsideCrypto.NET.Models.JsonRPC.CreateQueryRun;
using FlipsideCrypto.NET.Models.JsonRPC.GetQueryRun;
using FlipsideCrypto.NET.Models.JsonRPC.GetQueryRunResults;
using FlipsideCrypto.NET.Models.ValueObjects;
using System.Reflection;
using System.Text.Json;

namespace FlipsideCrypto.NET;
internal class FlipsideClient : IFlipsideClient
{
    private readonly JsonRPCClient _jsonRPCClient;
    private readonly FlipsideOptions _flipsideOptions;

    private readonly Uri _rpcUrl;

    public FlipsideClient(JsonRPCClient jsonRPCClient, FlipsideOptions flipsideOptions)
    {
        _jsonRPCClient = jsonRPCClient;
        _flipsideOptions = flipsideOptions;

        _rpcUrl = new Uri(flipsideOptions.BaseUri, "json-rpc");

        _jsonRPCClient.AddRequestHeader("x-api-key", _flipsideOptions.ApiKey);
    }

    public async Task<QueryRun> GetQueryRunAsync(QueryRunId runId, CancellationToken cancellationToken = default)
    {
        var payload = new GetQueryRunPayload()
        {
            QueryRunId = runId
        };

        var result = await _jsonRPCClient.SendAsync<GetQueryRunPayload, GetQueryRunResult>(_rpcUrl, payload, cancellationToken);
        return result.QueryRun;
    }

    public async Task<QueryRunId> CreateQueryRunAsync(string sql, CancellationToken cancellationToken = default)
    {
        var payload = new CreateQueryRunPayload()
        {
            ResultTTLHours = 5,
            MaxAgeMinutes = 15,
            Sql = sql,
            Tags = new Dictionary<string, string>(),
            DataSource = "snowflake-default",
            DataProvider = "flipside"
        };

        var result = await _jsonRPCClient.SendAsync<CreateQueryRunPayload, CreateQueryRunResult>(_rpcUrl, payload, cancellationToken);
        return result.QueryRun.Id;
    }

    public async Task<TModel[]> GetQueryRunResults<TModel>(QueryRunId runId,
        int page = 1, int pageSize = 10000, CancellationToken cancellationToken = default)
        where TModel : class, new()
    {
        var payload = new GetQueryRunResultsPayload()
        {
            QueryRunId = runId,
            Format = "csv",
            Page = new QueryResultsPage(page, pageSize)
        };

        var results = await _jsonRPCClient.SendAsync<GetQueryRunResultsPayload, GetQueryRunCSVResultsResult>(
            _rpcUrl, payload, cancellationToken);

        var output = new TModel[results.Rows.Length];
        var properties = typeof(TModel).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
        var colNames = results.ColumnNames.ToList();

        int[] mappingIndices = new int[properties.Length];

        for(int i = 0; i < properties.Length; i++)
        {
            var property = properties[i];
            int idx = colNames.FindIndex(x => x.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase));

            if(idx == -1)
            {
                throw new InvalidOperationException(
                    $"Could not cast result set to {nameof(TModel)}. Rows do not include column {property.Name}");
            }

            mappingIndices[i] = idx;
        }

        for(int i = 0; i < results.Rows.Length; i++)
        {
            var row = results.Rows[i];
            output[i] = Activator.CreateInstance<TModel>();

            for(int j = 0; j < properties.Length; j++)
            {
                var property = properties[j];
                int mappingIndex = mappingIndices[j];

                property.SetValue(output[i], JsonSerializer.Deserialize(row[mappingIndex], property.PropertyType));
            }
        }

        return output;
    }

    public async Task<QueryRun> CancelQueryAsync(QueryRunId runId, CancellationToken cancellationToken = default)
    {
        var payload = new CancelQueryRunPayload()
        {
            QueryRunId = runId
        };

        var result = await _jsonRPCClient.SendAsync<CancelQueryRunPayload, CancelQueryRunResult>(_rpcUrl, payload, cancellationToken);
        return result.CancelledQueryRun;
    }
}
