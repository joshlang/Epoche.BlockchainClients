namespace Epoche.BlockchainClients.Satoshi;

public class SatoshiBlockSummary
{
    [JsonPropertyName("hash")]
    public string Hash { get; set; } = default!;

    [JsonPropertyName("confirmations")]
    public long Confirmations { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("strippedsize")]
    public long? StrippedSize { get; set; }

    [JsonPropertyName("weight")]
    public long Weight { get; set; }

    [JsonPropertyName("height")]
    public long Height { get; set; }

    [JsonPropertyName("version")]
    public long Version { get; set; }

    [JsonPropertyName("versionHex")]
    public string VersionBytes { get; set; } = default!;

    [JsonPropertyName("merkleroot")]
    public string MerkleRoot { get; set; } = default!;

    string? previousBlockHash;
    [JsonPropertyName("previousblockhash")]
    public string PreviousBlockHash
    {
        get => previousBlockHash ??= new string('0', 64); // to simplify handling of genesis block
        set => previousBlockHash = value;
    }

    [JsonPropertyName("nextblockhash")]
    public string? NextBlockHash { get; set; } = default!;

    [JsonPropertyName("time")]
    public long Time { get; set; }

    [JsonPropertyName("mediantime")]
    public long MedianTime { get; set; }

    [JsonPropertyName("nonce")]
    public long Nonce { get; set; }

    [JsonPropertyName("bits")]
    public string Bits { get; set; } = default!;

    [JsonPropertyName("difficulty")]
    public decimal Difficulty { get; set; }

    [JsonPropertyName("chainwork")]
    public string? ChainWork { get; set; } = default!;

    [JsonPropertyName("tx")]
    public string[] TransactionHashes { get; set; } = default!;

    [JsonPropertyName("auxpow")]
    public SatoshiAuxPow? AuxilliaryProofOfWork { get; set; }
}
