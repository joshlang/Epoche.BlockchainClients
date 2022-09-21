namespace Epoche.BlockchainClients.Ethereum;

public class EthereumBlockData : EthereumBlockBase
{
    [JsonPropertyName("transactions")]
    public EthereumTransaction[] Transactions { get; set; } = default!;
}
