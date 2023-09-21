using Epoche.BlockchainClients.Ethereum;

namespace Epoche.BlockchainClients.Harmony;
public class HarmonyTransaction : EthereumTransaction
{
    [JsonPropertyName("timestamp")]
    [JsonConverter(typeof(EthereumHexInt64Converter))]
    public long Timestamp { get; set; }
}
