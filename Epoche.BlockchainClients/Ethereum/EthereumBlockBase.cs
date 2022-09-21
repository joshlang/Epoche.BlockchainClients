using System.Numerics;

namespace Epoche.BlockchainClients.Ethereum;

public class EthereumBlockBase
{
    [JsonPropertyName("number")]
    [JsonConverter(typeof(EthereumHexInt64Converter))]
    public long BlockNumber { get; set; }

    [JsonPropertyName("hash")]
    public string Hash { get; set; } = default!;

    [JsonPropertyName("parentHash")]
    public string ParentHash { get; set; } = default!;

    [JsonPropertyName("nonce")]
    public string Nonce { get; set; } = default!;

    [JsonPropertyName("sha3Uncles")]
    public string Sha3Uncles { get; set; } = default!;

    [JsonPropertyName("logsBloom")]
    public string LogsBloomFilter { get; set; } = default!;

    [JsonPropertyName("transactionsRoot")]
    public string TransactionsRoot { get; set; } = default!;

    [JsonPropertyName("stateRoot")]
    public string StateRoot { get; set; } = default!;

    [JsonPropertyName("receiptsRoot")]
    public string ReceiptsRoot { get; set; } = default!;

    [JsonPropertyName("miner")]
    public string MinerAddress { get; set; } = default!;

    [JsonPropertyName("difficulty")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger Difficulty { get; set; }

    [JsonPropertyName("totalDifficulty")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger TotalDifficulty { get; set; }

    [JsonPropertyName("size")]
    [JsonConverter(typeof(EthereumHexInt32Converter))]
    public int Size { get; set; }

    [JsonPropertyName("gasLimit")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger GasLimit { get; set; }

    [JsonPropertyName("gasUsed")]
    [JsonConverter(typeof(EthereumHexInt64Converter))]
    public long GasUsed { get; set; }

    [JsonPropertyName("timestamp")]
    [JsonConverter(typeof(EthereumHexInt64Converter))]
    public long Timestamp { get; set; }

    [JsonPropertyName("uncles")]
    public string[] UncleHashes { get; set; } = default!;
}
