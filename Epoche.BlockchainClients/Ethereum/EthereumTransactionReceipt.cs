using System.Numerics;

namespace Epoche.BlockchainClients.Ethereum;
public class EthereumTransactionReceipt
{
    [JsonPropertyName("transactionHash")]
    public string Hash { get; set; } = default!;

    [JsonPropertyName("transactionIndex")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger? TransactionIndex { get; set; }

    [JsonPropertyName("from")]
    public string FromAddress { get; set; } = default!;

    [JsonPropertyName("to")]
    public string? ToAddress { get; set; }

    [JsonPropertyName("blockHash")]
    public string BlockHash { get; set; } = default!;

    [JsonPropertyName("blockNumber")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger BlockNumber { get; set; }

    [JsonPropertyName("cumulativeGasUsed")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger CumulativeGasUsed { get; set; }

    [JsonPropertyName("gasUsed")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger GasUsed { get; set; }

    [JsonPropertyName("contractAddress")]
    public string? ContractAddress { get; set; }

    [JsonPropertyName("logs")]
    public EthereumLog[] Logs { get; set; } = default!;

    [JsonPropertyName("value")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger Value { get; set; }

    [JsonPropertyName("v")]
    public string V { get; set; } = default!;

    [JsonPropertyName("r")]
    public string R { get; set; } = default!;

    [JsonPropertyName("s")]
    public string S { get; set; } = default!;

    [JsonPropertyName("type")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger TypeId { get; set; }
}
