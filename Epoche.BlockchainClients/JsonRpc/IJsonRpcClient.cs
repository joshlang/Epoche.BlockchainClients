namespace Epoche.BlockchainClients.JsonRpc;

public interface IJsonRpcClient
{
    Task<JsonRpcBatchResult<T>> BatchRequestAsync<T>(string method, IEnumerable<object> requests, JsonRpcRequestOptions? requestOptions, CancellationToken cancellationToken = default);
    Task<JsonRpcResult<JsonElement>> RequestAsync(string method, object? request, JsonRpcRequestOptions? requestOptions, CancellationToken cancellationToken = default);
    Task<JsonRpcResult<T>> RequestAsync<T>(string method, object? request, JsonRpcRequestOptions? requestOptions, CancellationToken cancellationToken = default) where T : class;
    Task<JsonRpcResult<T>> RequestValueAsync<T>(string method, object? request, JsonRpcRequestOptions? requestOptions, CancellationToken cancellationToken = default) where T : struct;
}
