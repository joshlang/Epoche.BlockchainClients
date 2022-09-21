namespace Epoche.BlockchainClients.Satoshi;

public class SatoshiUnspentInfo
{
    [JsonPropertyName("bestblock")]
    public string? BestBlock { get; set; }

    [JsonPropertyName("confirmations")]
    public long Confirmations { get; set; }

    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    [JsonPropertyName("coinbase")]
    public bool Coinbase { get; set; }

    [JsonPropertyName("scriptPubKey")]
    public SatoshiScriptPubKey ScriptPubKey { get; set; } = default!;
}
