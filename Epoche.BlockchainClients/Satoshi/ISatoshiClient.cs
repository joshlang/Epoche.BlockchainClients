namespace Epoche.BlockchainClients.Satoshi;

public interface ISatoshiClient : IBlockchainClient
{
    Task<SatoshiValidateAddress[]> GetPublicAddressInfoAsync(IEnumerable<string> addresses, CancellationToken cancellationToken = default);
    Task<int> GetTransactionSizeWithoutInputsAsync(IEnumerable<string> outputAddresses, CancellationToken cancellationToken = default);
    Task<SatoshiNetworkInfo> GetNetworkInfoAsync(CancellationToken cancellationToken = default);
    Task<SatoshiTransaction[]> GetTransactionDetailsAsync(IEnumerable<string> hashes, bool ignoreMissing, CancellationToken cancellationToken = default);
    Task<string[]> GetMempoolTransactionHashesAsync(CancellationToken cancellationToken = default);
    Task<SatoshiTransactionFeeInfo[]> GetTransactionFeesAsync(IEnumerable<string> hashes, bool ignoreMissing, CancellationToken cancellationToken = default);
    Task<int> GetMaxBlockSizeAsync(CancellationToken cancellationToken = default);
    Task<SatoshiUnspentInfo?[]> GetUnspentInfoAsync(IEnumerable<(string Hash, int Index)> transactionReferences, CancellationToken cancellationToken = default);
    Task<SatoshiSignTransactionResult> CreateSignedTransactionAsync(IEnumerable<(string Hash, int Index)> inputs, IEnumerable<(string Address, decimal Value)> outputs, IEnumerable<string> privateKeyAddresses, CancellationToken cancellationToken = default);
    Task<SatoshiTransaction> DecodeTransactionAsync(string rawTransaction, CancellationToken cancellationToken = default);
    Task<TransactionInfo[]> GetTransactionsAsync(IEnumerable<string> hashes, bool ignoreMissing, CancellationToken cancellationToken = default);
    Task<string?> GetRawBlockAsync(string hash, CancellationToken cancellationToken = default);
    Task<string[]> DecodeScriptAddressesAsync(string script, CancellationToken cancellationToken = default);
    Task<string[][]> DecodeScriptAddressesAsync(string[] scripts, CancellationToken cancellationToken = default);
    Task ImportPrivateKeyAsync(string privateKey, bool rescan, string label = "", CancellationToken cancellationToken = default);
}
