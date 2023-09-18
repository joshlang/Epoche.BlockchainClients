namespace Epoche.BlockchainClients.Near;
public class NearBlockResult
{
    [JsonPropertyName("author")]
    public string Author { get; init; } = default!;
    [JsonPropertyName("header")]
    public NearBlockHeader Header { get; init; } = default!;
}
