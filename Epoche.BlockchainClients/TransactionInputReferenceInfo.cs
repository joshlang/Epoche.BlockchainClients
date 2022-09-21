namespace Epoche.BlockchainClients;

public class TransactionInputReferenceInfo
{
    public readonly string Hash;
    public readonly string Index;

    public TransactionInputReferenceInfo(
        string hash,
        string index)
    {
        Hash = hash ?? throw new ArgumentNullException(nameof(hash));
        Index = index ?? throw new ArgumentNullException(nameof(index));
    }
}
