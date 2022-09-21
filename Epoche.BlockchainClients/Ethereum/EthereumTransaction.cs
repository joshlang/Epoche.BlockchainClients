using System.Numerics;

namespace Epoche.BlockchainClients.Ethereum;

public class EthereumTransaction
{
    [JsonPropertyName("blockHash")]
    public string? BlockHash { get; set; }

    [JsonPropertyName("blockNumber")]
    [JsonConverter(typeof(EthereumHexInt64Converter))]
    public long BlockNumber { get; set; }

    [JsonPropertyName("from")]
    public string FromAddress { get; set; } = default!;

    [JsonPropertyName("gas")]
    [JsonConverter(typeof(EthereumHexInt64Converter))]
    public long GasUnitsProvided { get; set; }

    [JsonPropertyName("gasPrice")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger GasPricePerUnitE18 { get; set; }

    [JsonPropertyName("hash")]
    public string Hash { get; set; } = default!;

    [JsonPropertyName("input")]
    public string Input { get; set; } = default!;

    [JsonPropertyName("nonce")]
    [JsonConverter(typeof(EthereumHexInt64Converter))]
    public long Nonce { get; set; }

    /// <summary>
    /// "To" address is null for contract creation
    /// </summary>
    [JsonPropertyName("to")]
    public string? ToAddress { get; set; } = default!;

    [JsonPropertyName("transactionIndex")]
    [JsonConverter(typeof(EthereumHexInt32Converter))]
    public int TransactionIndex { get; set; }

    [JsonPropertyName("value")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger Value { get; set; }

    [JsonPropertyName("v")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger SignatureRecoveryId { get; set; }

    [JsonPropertyName("r")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger SignatureR { get; set; }

    [JsonPropertyName("s")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger SignatureS { get; set; }
}
