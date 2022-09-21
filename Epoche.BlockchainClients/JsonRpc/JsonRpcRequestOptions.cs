namespace Epoche.BlockchainClients.JsonRpc;

public class JsonRpcRequestOptions
{
    public bool UseVersion2 { get; set; } = true;
    public JsonSerializerOptions? SerializerOptions { get; set; }
    public int MaxRequestsPerBatch { get; set; } = 250;
}
