namespace Epoche.BlockchainClients.Near;
public class NearTransactionResult
{
    [JsonPropertyName("receipts_outcome")]
    public NearReceiptOutcome[] ReceiptsOutcome { get; init; } = default!;
    [JsonPropertyName("transaction_outcome")]
    public NearTransactionOutcome TransactionOutcome { get; init; } = default!;

    [JsonIgnore] public string Hash => TransactionOutcome.Id;
}
