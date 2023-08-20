namespace Epoche.BlockchainClients;

public class TransactionOutputInfo
{
    public required string Address { get; init; }
    public required decimal Value { get; init; }
    public required string Index { get; init; }
}
