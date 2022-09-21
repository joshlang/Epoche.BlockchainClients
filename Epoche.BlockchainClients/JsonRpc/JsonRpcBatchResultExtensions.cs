namespace Epoche.BlockchainClients.JsonRpc;

static class JsonRpcBatchResultExtensions
{
    public static async Task<JsonRpcResult<T>[]> ThrowOnError<T>(this Task<JsonRpcBatchResult<T>> resultTask)
    {
        var result = await resultTask.ConfigureAwait(false);
        result.FirstError?.Throw();
        return result.Results;
    }
    public static JsonRpcResult<T>[] ThrowOnError<T>(this JsonRpcBatchResult<T> result)
    {
        result.FirstError?.Throw();
        return result.Results;
    }
}
