namespace Epoche.BlockchainClients;

public class TransactionInfo
{
    public required string Hash { get; init; }
    public required DateTime Date { get; init; }

    public required TransactionInputReferenceInfo[] InputReferences { get; init; }
    public required TransactionOutputInfo[] Outputs { get; init; }

    internal static TransactionInfo Create(
        DateTime date,
        string hash,
        IEnumerable<TransactionInputReferenceInfo>? inputReferences,
        IEnumerable<TransactionOutputInfo> outputs) => new TransactionInfo
        {
            Date = date,
            Hash = hash ?? throw new ArgumentNullException(nameof(hash)),
            InputReferences = inputReferences.EmptyIfNull().ToArray(),
            Outputs = outputs?.ToArray() ?? throw new ArgumentNullException(nameof(outputs))
        };
}
