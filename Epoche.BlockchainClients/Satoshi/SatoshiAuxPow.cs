namespace Epoche.BlockchainClients.Satoshi;

public class SatoshiAuxPow
{
    [JsonPropertyName("tx")]
    public SatoshiTransaction Transaction { get; set; } = default!;

    [JsonPropertyName("index")]
    public long Index { get; set; }

    [JsonPropertyName("chainindex")]
    public long ChainIndex { get; set; }

    [JsonPropertyName("chainmerklebranch")]
    public string[] ChainMerkleBranch { get; set; } = Array.Empty<string>();

    [JsonPropertyName("merklebranch")]
    public string[] MerkleBranch { get; set; } = Array.Empty<string>();

    [JsonPropertyName("parentblock")]
    public string ParentBlock { get; set; } = default!;
}
