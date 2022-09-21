namespace Epoche.BlockchainClients.Ethereum;

public class EthereumBlockSummary : EthereumBlockBase
{
    [JsonPropertyName("transactions")]
    public string[] TransactionHashes { get; set; } = default!;
}
