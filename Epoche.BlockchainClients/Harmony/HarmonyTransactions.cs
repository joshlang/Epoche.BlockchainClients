namespace Epoche.BlockchainClients.Harmony;
class HarmonyTransactions
{
    [JsonPropertyName("transactions")]
    public HarmonyTransaction[] Transactions { get; set; } = default!;
}
