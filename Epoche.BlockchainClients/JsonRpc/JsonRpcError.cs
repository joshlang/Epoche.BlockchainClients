namespace Epoche.BlockchainClients.JsonRpc;

public sealed class JsonRpcError
{
    [JsonPropertyName("code")] public int Code { get; set; }
    [JsonPropertyName("message")] public string? Message { get; set; }
    [JsonPropertyName("data")] public JsonDocument? Data { get; set; }

    public override string ToString() => $"[Error {Code}: {Message}]";
    internal void Throw() => throw new JsonRpcException(error: this);
}
