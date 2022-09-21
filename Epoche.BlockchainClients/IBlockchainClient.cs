namespace Epoche.BlockchainClients;

public interface IBlockchainClient
{
    bool SupportsGetRawTransaction { get; }

    Task<long> GetBlockCountAsync(CancellationToken cancellationToken = default);
    Task<BlockInfo?> GetBlockAsync(long height, CancellationToken cancellationToken = default);
    Task<string?> GetBlockHashAsync(long height, CancellationToken cancellationToken = default);
    Task<BlockInfo?> GetBlockAsync(string hash, CancellationToken cancellationToken = default);
    Task<BlockInfo> GetBestBlockAsync(CancellationToken cancellationToken = default);

    Task<bool> ValidatePublicAddressAsync(string address, CancellationToken cancellationToken = default);
    Task<bool[]> ValidatePublicAddressesAsync(IEnumerable<string> addresses, CancellationToken cancellationToken = default);

    Task<string?> GetRawTransactionAsync(string hash, CancellationToken cancellationToken = default);
    Task<bool> SendRawTransactionAsync(string rawTransaction, CancellationToken cancellationToken = default);
    Task<long?> GetTransactionConfirmationsAsync(string hash, CancellationToken cancellationToken = default);

    Task<TransactionInfo[]?> GetBlockTransactionsAsync(string hash, CancellationToken cancellationToken = default);
}
