using FlipsideCrypto.NET.Common;
using FlipsideCrypto.NET.Configuration;
using FlipsideCrypto.NET.JsonRPC.Services;
using FlipsideCrypto.NET.Models;
using FlipsideCrypto.NET.Models.JsonRPC.CancelQueryRun;
using FlipsideCrypto.NET.Models.JsonRPC.CreateQueryRun;
using FlipsideCrypto.NET.Models.JsonRPC.GetQueryRun;
using FlipsideCrypto.NET.Models.JsonRPC.GetQueryRunResults;
using FlipsideCrypto.NET.Models.ValueObjects;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;
using System.Reflection;
using System.Runtime.CompilerServices;
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

    private readonly AsyncRetryPolicy<QueryRun> _runPolicy = Policy
        .HandleResult<QueryRun>(x => x.State != QueryState.Success && x.State != QueryState.Failed && x.State != QueryState.Cancelled)
        .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(3), 100));

    private async Task<QueryRun> RunQueryAndWaitForCompletionAsync(string sql, CancellationToken cancellationToken = default)
    {
        var runId = await CreateQueryRunAsync(sql, cancellationToken);
        var result = await _runPolicy.ExecuteAndCaptureAsync((cToken) => GetQueryRunAsync(runId, cToken), cancellationToken);

        if(result.Outcome != OutcomeType.Successful)
        {
            if(result.FaultType.HasValue && result.FaultType.Value == FaultType.UnhandledException)
            {
                throw result.FinalException;
            }

            throw new Exception("Unknown error occured while running query");
        }

        return result.Result.State switch
        {
            QueryState.Success => result.Result,
            QueryState.Failed => throw new Exception($"Query execution failed! {result.Result.ErrorName}: {result.Result.ErrorMessage}"),
            QueryState.Cancelled => throw new OperationCanceledException("Query execution has been cancelled"),
            _ => throw new ImpossibleException("Query execution completed but QueryState not in any completed state"),
        };
    }

    public async Task<TModel[]> RunQueryAsync<TModel>(string sql,
        int page = 1, int pageSize = 10000, CancellationToken cancellationToken = default)
        where TModel : class, new()
    {
        var queryRun = await RunQueryAndWaitForCompletionAsync(sql, cancellationToken);
        return await GetQueryRunResults<TModel>(queryRun.Id, page, pageSize, cancellationToken);
    }

    public async IAsyncEnumerable<TModel[]> RunBatchedQueryAsync<TModel>(string sql,
        int batchSize = 10000, [EnumeratorCancellation] CancellationToken cancellationToken = default) where TModel : class, new()
    {
        var queryRun = await RunQueryAndWaitForCompletionAsync(sql, cancellationToken);

        if(!queryRun.RowCount.HasValue)
        {
            throw new ImpossibleException("Query successful but RowCount not set");
        }

        await foreach(var batch in GetBatchedRunResultsEnumerator<TModel>(queryRun.Id, queryRun.RowCount.Value, batchSize, cancellationToken))
        {
            yield return batch;
        }
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

    public async IAsyncEnumerable<TModel[]> GetBatchedQueryRunResultsAsync<TModel>(QueryRunId runId,
        int batchSize = 10000, [EnumeratorCancellation] CancellationToken cancellationToken = default) 
        where TModel : class, new()
    {
        var queryRun = await GetQueryRunAsync(runId, cancellationToken);

        if(!queryRun.RowCount.HasValue)
        {
            throw new ImpossibleException("Query successful but RowCount not set");
        }

        await foreach(var batch in GetBatchedRunResultsEnumerator<TModel>(runId, queryRun.RowCount.Value, batchSize, cancellationToken))
        {
            yield return batch;
        }
    }

    private async IAsyncEnumerable<TModel[]> GetBatchedRunResultsEnumerator<TModel>(QueryRunId runId, 
        int rowCount, int batchSize,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TModel : class, new()
    {
        int pageCount = Math.Max(1, rowCount / batchSize);

        for(int i = 0; i < pageCount; i++)
        {
            var results = await GetQueryRunResults<TModel>(runId, i + 1, batchSize, cancellationToken);
            yield return results;
        }
    }
}
