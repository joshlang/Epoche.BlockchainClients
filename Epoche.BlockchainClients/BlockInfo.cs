namespace Epoche.BlockchainClients;

public class BlockInfo
{
    public readonly string Hash, PreviousHash;
    public readonly long Height;

    public BlockInfo(
        long height,
        string hash,
        string previousHash)
    {
        Height = height;
        Hash = hash ?? throw new ArgumentNullException(nameof(hash));
        PreviousHash = previousHash ?? throw new ArgumentNullException(nameof(previousHash));
    }
}
