namespace Epoche.BlockchainClients.Satoshi;

public class SatoshiScriptPubKey
{
    [JsonPropertyName("asm")]
    public string Asm { get; set; } = default!;

    [JsonPropertyName("hex")]
    public string RawScript { get; set; } = default!;

    [JsonPropertyName("reqSigs")]
    public int RequireSignatureCount { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;

    [JsonPropertyName("addresses")]
    public string[] Addresses { get; set; } = Array.Empty<string>();
}
