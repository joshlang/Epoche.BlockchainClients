namespace Epoche.BlockchainClients.Satoshi;

public class SatoshiValidateAddress
{
    [JsonPropertyName("isvalid")]
    public bool IsValid { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("scriptPubKey")]
    public string? ScriptPubKey { get; set; }

    [JsonPropertyName("isscript")]
    public bool IsScript { get; set; }

    [JsonPropertyName("iswitness")]
    public bool IsWitness { get; set; }

    [JsonPropertyName("witness_version")]
    public int? WitnessVersion { get; set; }

    [JsonPropertyName("witness_program")]
    public string? WitnessProgram { get; set; }
}
