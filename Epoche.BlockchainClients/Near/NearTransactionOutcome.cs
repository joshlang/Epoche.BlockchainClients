namespace Epoche.BlockchainClients.Near;
public class NearTransactionOutcome
{
    public class OutcomeInner
    {
        public class OutcomeStatus
        {
            public string SuccessReceiptId { get; init; } = default!;
        }
        [JsonPropertyName("executor_id")]
        public string Executor { get; init; } = default!;
        [JsonPropertyName("gas_burnt")]
        public long GasBurnt { get; init; }
        [JsonPropertyName("tokens_burnt")]
        public string TokensBurnt { get; init; } = default!;
        [JsonPropertyName("status")]
        public OutcomeStatus Status { get; init; } = default!;
    }

    [JsonPropertyName("block_hash")]
    public string BlockHash { get; init; } = default!;

    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;

    [JsonPropertyName("outcome")]
    public OutcomeInner Outcome { get; init; } = default!;
}
