namespace Epoche.BlockchainClients;

public class TransactionOutputInfo
{
    public readonly string Address;
    public readonly decimal Value;
    public readonly string Index;

    public TransactionOutputInfo(
        string address,
        decimal value,
        string index)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Index = index ?? throw new ArgumentNullException(nameof(index));
        Value = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
    }
}
