using System.Numerics;

namespace Epoche.BlockchainClients.Ethereum;

public class EthereumCallTrace
{
    public class CallTraceResults
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = default!;

        [JsonPropertyName("from")]
        public string From { get; set; } = default!;

        [JsonPropertyName("to")]
        public string To { get; set; } = default!;

        [JsonPropertyName("value")]
        [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
        public BigInteger Value { get; set; }

        [JsonPropertyName("gas")]
        [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
        public BigInteger GasProvided { get; set; }

        [JsonPropertyName("gasUsed")]
        [JsonConverter(typeof(EthereumHexBigIntegerConverter))]
        public BigInteger GasUsed { get; set; }

        [JsonPropertyName("input")]
        public string Input { get; set; } = default!;

        [JsonPropertyName("output")]
        public string Output { get; set; } = default!;

        [JsonPropertyName("time")]
        public string? Time { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("calls")]
        public CallTraceResults[] Calls { get; set; } = Array.Empty<CallTraceResults>();
    }

    [JsonPropertyName("result")]
    public CallTraceResults Result { get; set; } = default!;
}
