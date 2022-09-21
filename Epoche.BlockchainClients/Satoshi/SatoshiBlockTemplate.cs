namespace Epoche.BlockchainClients.Satoshi;

public class SatoshiBlockTemplate
{
    [JsonPropertyName("capabilities")]
    public string[] Capabilities { get; set; } = Array.Empty<string>();

    [JsonPropertyName("version")]
    public long Version { get; set; }

    [JsonPropertyName("previousblockhash")]
    public string PreviousBlockHash { get; set; } = default!;

    [JsonPropertyName("coinbasevalue")]
    public long CoinbaseValue { get; set; }

    [JsonPropertyName("longpollid")]
    public string? LongPollId { get; set; }

    [JsonPropertyName("mintime")]
    public long MinTime { get; set; }

    [JsonPropertyName("mutable")]
    public string[] Mutable { get; set; } = Array.Empty<string>();

    [JsonPropertyName("noncerange")]
    public string? NonceRange { get; set; }

    [JsonPropertyName("sigoplimit")]
    public int SigopLimit { get; set; }

    [JsonPropertyName("sizelimit")]
    public int SizeLimit { get; set; }

    [JsonPropertyName("curtime")]
    public long CurrentTime { get; set; }

    [JsonPropertyName("bits")]
    public string Bits { get; set; } = default!;

    [JsonPropertyName("height")]
    public long Height { get; set; }
}
