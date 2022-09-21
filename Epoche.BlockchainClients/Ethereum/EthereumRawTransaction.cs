namespace Epoche.BlockchainClients.Ethereum;

public class EthereumRawTransaction
{
    [JsonPropertyName("raw")]
    public string RawTransaction { get; set; } = default!;

    [JsonPropertyName("tx")]
    public EthereumTransaction Transaction { get; set; } = default!;
}
