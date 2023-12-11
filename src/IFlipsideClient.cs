using FlipsideCrypto.NET.Models;
using FlipsideCrypto.NET.Models.ValueObjects;

namespace FlipsideCrypto.NET;
public interface IFlipsideClient
{
    public Task<QueryRun> GetQueryRunAsync(QueryRunId runId, CancellationToken cancellationToken = default);
    public Task<QueryRunId> CreateQueryRunAsync(string sql, CancellationToken cancellationToken = default);
    public Task<TModel[]> GetQueryRunResults<TModel>(QueryRunId runId,
        int page = 1, int pageSize = 10000, CancellationToken cancellationToken = default)
        where TModel : class, new();
    public Task<TModel[]> RunQueryAsync<TModel>(string sql, 
        int page = 1, int pageSize = 10000, CancellationToken cancellationToken = default)
        where TModel : class, new();
    public Task<QueryRun> CancelQueryAsync(QueryRunId runId, CancellationToken cancellationToken = default);
}
