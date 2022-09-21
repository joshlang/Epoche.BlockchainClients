namespace Epoche.BlockchainClients;

public class TransactionInfo
{
    public readonly string Hash;

    public readonly TransactionInputReferenceInfo[] InputReferences;
    public readonly TransactionOutputInfo[] Outputs;

    public TransactionInfo(
        string hash,
        IEnumerable<TransactionInputReferenceInfo>? inputReferences,
        IEnumerable<TransactionOutputInfo> outputs)
    {
        Hash = hash ?? throw new ArgumentNullException(nameof(hash));
        InputReferences = inputReferences.EmptyIfNull().ToArray();
        Outputs = outputs?.ToArray() ?? throw new ArgumentNullException(nameof(outputs));
    }
}
