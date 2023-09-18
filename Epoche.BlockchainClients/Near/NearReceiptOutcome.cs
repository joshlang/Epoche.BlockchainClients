namespace Epoche.BlockchainClients.Near;
public class NearReceiptOutcome
{
    [JsonPropertyName("block_hash")]
    public string BlockHash { get; init; } = default!;
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;
}
