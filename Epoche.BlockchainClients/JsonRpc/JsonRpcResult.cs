namespace Epoche.BlockchainClients.JsonRpc;

public sealed class JsonRpcResult<TResult>
{
    readonly TResult result = default!;
    public TResult Result => Ok ? result : throw new InvalidOperationException($"The {nameof(Result)} property cannot be accessed when {nameof(Error)} is not null");
    public JsonRpcError? Error { get; }
    public bool Ok => Error is null;

    public JsonRpcResult(TResult result)
    {
        this.result = result;
    }
    public JsonRpcResult(JsonRpcError error)
    {
        Error = error ?? throw new ArgumentNullException(nameof(error));
    }
}
