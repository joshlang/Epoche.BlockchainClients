namespace Epoche.BlockchainClients.Satoshi;

public class SatoshiTransaction
{
    [JsonPropertyName("txid")]
    public string StandardHash { get; set; } = default!;

    [JsonPropertyName("hash")]
    public string WitnessHash { get; set; } = default!;

    [JsonPropertyName("version")]
    public long Version { get; set; } = 1;

    [JsonPropertyName("size")]
    public int Size { get; set; }

    int? virtualSize;
    [JsonPropertyName("vsize")]
    public int VirtualSize
    {
        get => virtualSize ?? Size;
        set => virtualSize = value;
    }

    [JsonPropertyName("weight")]
    public int Weight { get; set; }

    [JsonPropertyName("locktime")]
    public long Locktime { get; set; }

    [JsonPropertyName("hex")]
    public string RawTransaction { get; set; } = default!;

    [JsonPropertyName("blockhash")]
    public string? BlockHash { get; set; }

    [JsonPropertyName("confirmations")]
    public long Confirmations { get; set; }

    [JsonPropertyName("time")]
    public long Time { get; set; }

    [JsonPropertyName("blocktime")]
    public long BlockTime { get; set; }

    [JsonPropertyName("vin")]
    public SatoshiTransactionInput[] Inputs { get; set; } = default!;

    [JsonPropertyName("vout")]
    public SatoshiTransactionOutput[] Outputs { get; set; } = default!;

    internal TransactionInfo ToTransactionInfo() => new(
        hash: StandardHash,
        inputReferences: Inputs
            .Where(i => i.Coinbase is null)
            .Select(i => new TransactionInputReferenceInfo(hash: i.Hash, index: i.Index.ToString())),
        outputs: Outputs
            .Where(o => o.Script?.Addresses.Length == 1)
            .Select(o => new TransactionOutputInfo(
                address: o.Script.Addresses[0],
                value: o.Value,
                index: o.Index.ToString())));
}
