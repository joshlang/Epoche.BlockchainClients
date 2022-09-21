namespace Epoche.BlockchainClients.JsonRpc;

sealed class RawJsonRpcResult<T>
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("result")]
    public T Result { get; set; } = default!;

    [JsonPropertyName("error")]
    public JsonRpcError? Error { get; set; }

    public JsonRpcResult<T> ToRpcResult() => Error != null ? new JsonRpcResult<T>(Error) : new JsonRpcResult<T>(Result);
}
