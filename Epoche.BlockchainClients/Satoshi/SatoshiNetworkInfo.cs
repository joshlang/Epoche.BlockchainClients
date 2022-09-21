namespace Epoche.BlockchainClients.Satoshi;

public class SatoshiNetworkInfo
{
    [JsonPropertyName("version")]
    public long Version { get; set; }

    [JsonPropertyName("subversion")]
    public string SubVersion { get; set; } = default!;

    [JsonPropertyName("protocolversion")]
    public long ProtocolVersion { get; set; }

    [JsonPropertyName("localservices")]
    public string LocalServices { get; set; } = default!;

    [JsonPropertyName("localrelay")]
    public bool LocalRelay { get; set; }

    [JsonPropertyName("timeoffset")]
    public int TimeOffset { get; set; }

    [JsonPropertyName("networkactive")]
    public bool NetworkActive { get; set; }

    [JsonPropertyName("connections")]
    public int Connections { get; set; }

    [JsonPropertyName("relayfee")]
    public decimal RelayFeePerKB { get; set; }

    [JsonPropertyName("incrementalfee")]
    public decimal IncrementalFee { get; set; }

    [JsonPropertyName("warnings")]
    public string Warnings { get; set; } = "";
}
