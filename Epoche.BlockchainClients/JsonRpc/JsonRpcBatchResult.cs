namespace Epoche.BlockchainClients.JsonRpc;

public sealed class JsonRpcBatchResult<TResult>
{
    public JsonRpcResult<TResult>[] Results { get; }
    JsonRpcError? firstError;
    public JsonRpcError? FirstError => firstError ??= (AllOk ? null : Results.Select(x => x.Error).FirstOrDefault(x => x != null));
    bool? allOk;
    public bool AllOk => allOk ??= Results.All(x => x.Ok);

    public JsonRpcBatchResult(JsonRpcResult<TResult>[] results)
    {
        Results = results ?? throw new ArgumentNullException(nameof(results));
    }
}
