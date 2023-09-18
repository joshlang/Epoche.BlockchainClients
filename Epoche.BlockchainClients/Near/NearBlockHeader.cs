namespace Epoche.BlockchainClients.Near;
public class NearBlockHeader
{
    [JsonPropertyName("height")]
    public long Height { get; init; }
    [JsonPropertyName("hash")]
    public string Hash { get; init; } = default!;
    [JsonPropertyName("prev_hash")]
    public string PreviousHash { get; init; } = default!;

    [JsonPropertyName("timestamp_nanosec")]
    public string TimestampNanoseconds { get; init; } = default!;

    [JsonIgnore] public DateTime Date => DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(TimestampNanoseconds) / 1000000).UtcDateTime;
}
