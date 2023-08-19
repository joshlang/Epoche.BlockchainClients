namespace Epoche.BlockchainClients;

public class TransactionInfo
{
    public readonly string Hash;
    public readonly DateTime Date;

    public readonly TransactionInputReferenceInfo[] InputReferences;
    public readonly TransactionOutputInfo[] Outputs;

    public TransactionInfo(
        DateTime date,
        string hash,
        IEnumerable<TransactionInputReferenceInfo>? inputReferences,
        IEnumerable<TransactionOutputInfo> outputs)
    {
        Date = date;
        Hash = hash ?? throw new ArgumentNullException(nameof(hash));
        InputReferences = inputReferences.EmptyIfNull().ToArray();
        Outputs = outputs?.ToArray() ?? throw new ArgumentNullException(nameof(outputs));
    }
}
