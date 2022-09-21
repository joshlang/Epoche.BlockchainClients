using System.Numerics;

namespace Epoche.BlockchainClients.Ethereum;

public class EthereumLog
{
    [JsonPropertyName("removed")]
    public bool Removed { get; set; }

    [JsonPropertyName("logIndex")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger? LogIndex { get; set; }

    [JsonPropertyName("transactionIndex")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger? TransactionIndex { get; set; }

    [JsonPropertyName("transactionHash")]
    public string TransactionHash { get; set; } = default!;

    [JsonPropertyName("blockHash")]
    public string BlockHash { get; set; } = default!;

    [JsonPropertyName("blockNumber")]
    [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
    public BigInteger? BlockNumber { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; } = default!;

    [JsonPropertyName("data")]
    public string Data { get; set; } = default!;

    [JsonPropertyName("topics")]
    public string[] Topics { get; set; } = default!;
}
