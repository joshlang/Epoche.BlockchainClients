namespace Epoche.BlockchainClients.Satoshi;

public class SatoshiSignTransactionResult
{
    [JsonPropertyName("hex")]
    public string RawTransaction { get; set; } = string.Empty;

    [JsonPropertyName("complete")]
    public bool Complete { get; set; }
}
