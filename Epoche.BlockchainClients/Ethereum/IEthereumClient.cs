using System.Numerics;

namespace Epoche.BlockchainClients.Ethereum;

public interface IEthereumClient : IBlockchainClient
{
    Task<EthereumBlockSummary> GetBlockSummaryAsync(string hash, CancellationToken cancellationToken = default);
    Task<EthereumBlockData> GetBlockDataAsync(string hash, CancellationToken cancellationToken = default);
    Task<EthereumBlockData> GetBlockDataAsync(long blockNumber, CancellationToken cancellationToken = default);
    Task<string> GetCodeAsync(string address, CancellationToken cancellationToken = default);
    Task<decimal> GetBalanceAsync(string address, CancellationToken cancellationToken = default);
    Task<decimal[]> GetBalancesAsync(IEnumerable<string> addresses, CancellationToken cancellationToken = default);
    Task<EthereumRawTransaction> CreateRawTransactionAsync(string privateKey, string fromAddress, string toAddress, decimal value, BigInteger gasPriceE18, int gasProvided, int nonce, CancellationToken cancellationToken = default);
    Task<int> GetTransactionCountAsync(string address, CancellationToken cancellationToken = default);
    Task<EthereumLog[]> GetBlockContractEventsAsync(long height, string contractAddress, CancellationToken cancellationToken = default);
    Task<EthereumLog[]> GetBlockContractEventsAsync(long height, string[] contractAddresses, CancellationToken cancellationToken = default);
    Task<EthereumLog[]> GetBlockContractEventsAsync(long minHeight, long maxHeight, string contractAddress, CancellationToken cancellationToken = default);
    Task<EthereumLog[]> GetBlockContractEventsAsync(long minHeight, long maxHeight, string[] contractAddresses, CancellationToken cancellationToken = default);
    Task<EthereumTransaction?> GetTransactionAsync(string hash, CancellationToken cancellationToken = default);
    Task<string> RawCallAsync(string address, string data, CancellationToken cancellationToken = default);
    Task<EthereumTransactionReceipt?> GetTransactionReceiptAsync(string hash, CancellationToken cancellationToken = default);
}
