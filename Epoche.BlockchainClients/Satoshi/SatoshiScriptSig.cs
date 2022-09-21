namespace Epoche.BlockchainClients.Satoshi;

public class SatoshiScriptSig
{
    [JsonPropertyName("asm")]
    public string Asm { get; set; } = default!;

    [JsonPropertyName("hex")]
    public string Script { get; set; } = default!;
}
