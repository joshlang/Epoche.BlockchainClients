namespace Epoche.BlockchainClients.Satoshi;

public class SatoshiTransactionOutput
{
    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    [JsonPropertyName("n")]
    public int Index { get; set; }

    [JsonPropertyName("scriptPubKey")]
    public SatoshiScriptPubKey Script { get; set; } = default!;
}
