namespace Epoche.BlockchainClients.Satoshi;

public class SatoshiTransactionInput
{
    string? hash;
    [JsonPropertyName("txid")]
    public string Hash
    {
        get => hash ??= new string('0', 64);
        set => hash = value;
    }

    [JsonPropertyName("vout")]
    public int Index { get; set; }

    [JsonPropertyName("sequence")]
    public long Sequence { get; set; }

    [JsonPropertyName("coinbase")]
    public string? Coinbase { get; set; }

    [JsonPropertyName("txinwitness")]
    public string[] Witnesses { get; set; } = Array.Empty<string>();

    [JsonPropertyName("scriptSig")]
    public SatoshiScriptSig? ScriptSig { get; set; }
}
