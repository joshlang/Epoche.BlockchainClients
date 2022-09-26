using System.Globalization;
using System.Numerics;
using Epoche.BlockchainClients.JsonRpc;

namespace Epoche.BlockchainClients.Ethereum;

public class EthereumClient : IEthereumClient
{
    public const decimal WeiPerEther = 1000000000000000000;
    const int AddressLength = 42;

    static readonly JsonRpcRequestOptions Options = new()
    {
        SerializerOptions = new JsonSerializerOptions
        {
            MaxDepth = 1024
        }
    };

    readonly IJsonRpcClient Client;

    public EthereumClient(IJsonRpcClient jsonRpcClient)
    {
        Client = jsonRpcClient ?? throw new ArgumentNullException(nameof(jsonRpcClient));
    }

    Task<JsonRpcResult<T>> RequestAsync<T>(string method, object? request, CancellationToken cancellationToken = default) where T : class => Client.RequestAsync<T>(method: method, request: request, requestOptions: Options, cancellationToken: cancellationToken);
    Task<JsonRpcResult<T>> RequestValueAsync<T>(string method, object? request, CancellationToken cancellationToken = default) where T : struct => Client.RequestValueAsync<T>(method: method, request: request, requestOptions: Options, cancellationToken: cancellationToken);

    static long HexToLong(string s)
    {
        if (s.StartsWith("0x") || s.StartsWith("0X"))
        {
            return long.Parse(s[2..], NumberStyles.HexNumber);
        }

        throw new InvalidOperationException("Number does not start with 0x");
    }

    static string FixHexNumber(string hex)
    {
        if (hex.Length <= 3)
        {
            return hex;
        }
        return hex.Replace("0x0", "0x");
    }
    internal static string ToHex(long num) => FixHexNumber($"0x{num:x}");
    internal static string ToHex(BigInteger num) => FixHexNumber($"0x{num:x}");

    public bool SupportsGetRawTransaction => true;

    public async Task<BlockInfo> GetBestBlockAsync(CancellationToken cancellationToken = default)
    {
        var block = await RequestAsync<EthereumBlockSummary>("eth_getBlockByNumber", new object[] { "latest", false }, cancellationToken).ThrowOnError().ConfigureAwait(false);
        return new BlockInfo(
            height: block.BlockNumber,
            hash: block.Hash,
            previousHash: block.ParentHash);
    }

    public async Task<BlockInfo?> GetBlockAsync(long height, CancellationToken cancellationToken = default)
    {
        var block = await RequestAsync<EthereumBlockSummary>("eth_getBlockByNumber", new object[] { ToHex(height), false }, cancellationToken).ThrowOnError().ConfigureAwait(false);
        if (block is null)
        {
            return null;
        }
        return new BlockInfo(
            height: block.BlockNumber,
            hash: block.Hash,
            previousHash: block.ParentHash);
    }

    public async Task<string?> GetBlockHashAsync(long height, CancellationToken cancellationToken = default)
    {
        var block = await GetBlockAsync(height, cancellationToken);
        return block?.Hash;
    }

    public async Task<BlockInfo?> GetBlockAsync(string hash, CancellationToken cancellationToken = default)
    {
        var block = await RequestAsync<EthereumBlockSummary>("eth_getBlockByHash", new object[] { hash, false }, cancellationToken).ThrowOnError().ConfigureAwait(false);
        if (block is null)
        {
            return null;
        }
        return new BlockInfo(
            height: block.BlockNumber,
            hash: block.Hash,
            previousHash: block.ParentHash);
    }

    public async Task<long> GetBlockCountAsync(CancellationToken cancellationToken = default)
    {
        var result = await RequestAsync<string>("eth_blockNumber", null, cancellationToken).ThrowOnError().ConfigureAwait(false);
        return HexToLong(result);
    }

    public Task<string?> GetRawTransactionAsync(string hash, CancellationToken cancellationToken = default) =>
        RequestAsync<string>("eth_getRawTransactionByHash", new object[] { hash }, cancellationToken).ThrowOnError()!;

    public async Task<long?> GetTransactionConfirmationsAsync(string hash, CancellationToken cancellationToken = default)
    {
        var transaction = await GetTransactionAsync(hash, cancellationToken);
        if (transaction is null)
        {
            return null;
        }
        if (transaction.BlockNumber == 0)
        {
            return 0;
        }
        var height = await GetBlockCountAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        if (height < transaction.BlockNumber)
        {
            return 0;
        }
        return height - transaction.BlockNumber + 1;
    }

    public async Task<EthereumTransaction?> GetTransactionAsync(string hash, CancellationToken cancellationToken = default) =>
        await RequestAsync<EthereumTransaction>("eth_getTransactionByHash", new[] { hash }, cancellationToken).ThrowOnError().ConfigureAwait(false);

    public async Task<bool> SendRawTransactionAsync(string rawTransaction, CancellationToken cancellationToken = default)
    {
        await RequestAsync<string>("eth_sendRawTransaction", new[] { rawTransaction }, cancellationToken).ConfigureAwait(false);
        return true;
    }

    static bool AddressOk(string? address)
    {
        if (address?.Length != AddressLength ||
            address[0] != '0' ||
            address[1] != 'x' ||
            address[2..].TryToHexBytes() is null)
        {
            return false;
        }
        return true;
    }
    public Task<bool> ValidatePublicAddressAsync(string address, CancellationToken cancellationToken = default)
    {
        if (!AddressOk(address))
        {
            return Task.FromResult(false);
        }
        return Task.FromResult(true);
    }

    public Task<bool[]> ValidatePublicAddressesAsync(IEnumerable<string> addresses, CancellationToken cancellationToken = default)
    {
        var results = addresses.Select(AddressOk).ToArray();
        return Task.FromResult(results);
    }

    IEnumerable<EthereumCallTrace.CallTraceResults> FlattenTraces(EthereumCallTrace.CallTraceResults start)
    {
        if (start.Error != null)
        {
            yield break;
        }
        yield return start;
        foreach (var call in start.Calls)
        {
            foreach (var trace in FlattenTraces(call))
            {
                yield return trace;
            }
        }
    }

    public async Task<TransactionInfo[]?> GetBlockTransactionsAsync(string hash, CancellationToken cancellationToken = default)
    {
        var block = await RequestAsync<EthereumBlockSummary>("eth_getBlockByHash", new object[] { hash, false }, cancellationToken).ThrowOnError().ConfigureAwait(false);
        if (block is null)
        {
            return null;
        }

        var traces = await RequestAsync<EthereumCallTrace[]>("debug_traceBlockByHash", new object[] { hash, new { tracer = "callTracer", timeout = "300s" } }, cancellationToken).ThrowOnError().ConfigureAwait(false);

        for (var x = 0; x < traces.Length; ++x)
        {
            if (traces[x].Result is null)
            {
                traces[x] = await RequestAsync<EthereumCallTrace>("debug_traceTransaction", new object[] { block.TransactionHashes[x], new { tracer = "callTracer" } }, cancellationToken).ThrowOnError().ConfigureAwait(false);
            }
        }

        var traceResults = traces
            .Select(x =>
            {
                var amountsByAddress = new Dictionary<string, decimal>();
                foreach (var result in FlattenTraces(x.Result).Where(r => r.Value > 0))
                {
                    var resultValue = (decimal)result.Value / WeiPerEther;
                    amountsByAddress.TryGetValue(result.From, out var val);
                    amountsByAddress[result.From] = val - resultValue;
                    amountsByAddress.TryGetValue(result.To, out val);
                    amountsByAddress[result.To] = val + resultValue;
                }
                foreach (var addr in amountsByAddress.Where(x => x.Value <= 0).ToList())
                {
                    amountsByAddress.Remove(addr.Key);
                }
                return amountsByAddress;
            })
            .ToArray();

        var transactions = block
            .TransactionHashes
            .Zip(traceResults, (txHash, traceResult) => new TransactionInfo(
                hash: txHash,
                inputReferences: null,
                outputs: traceResult
                    .OrderBy(x => x.Key)
                    .Select((x, i) => new TransactionOutputInfo(
                        address: x.Key,
                        value: x.Value,
                        index: i.ToString()))))
            .ToArray();

        return transactions;
    }

    public Task<EthereumBlockSummary> GetBlockSummaryAsync(string hash, CancellationToken cancellationToken = default) =>
        RequestAsync<EthereumBlockSummary>("eth_getBlockByHash", new object[] { hash, false }, cancellationToken).ThrowOnError();

    public Task<EthereumBlockData> GetBlockDataAsync(string hash, CancellationToken cancellationToken = default) =>
        RequestAsync<EthereumBlockData>("eth_getBlockByHash", new object[] { hash, true }, cancellationToken).ThrowOnError();

    public Task<EthereumBlockData> GetBlockDataAsync(long blockNumber, CancellationToken cancellationToken = default) =>
        RequestAsync<EthereumBlockData>("eth_getBlockByNumber", new object[] { ToHex(blockNumber), true }, cancellationToken).ThrowOnError();

    public Task<string> GetCodeAsync(string address, CancellationToken cancellationToken = default) =>
        RequestAsync<string>("eth_getCode", new object[] { address, "latest" }, cancellationToken).ThrowOnError();

    public async Task<decimal> GetBalanceAsync(string address, CancellationToken cancellationToken = default)
    {
        var balanceString = await RequestAsync<string>("eth_getBalance", new[] { address, "pending" }, cancellationToken: cancellationToken).ThrowOnError().ConfigureAwait(false);
        var balanceInt = EthereumHexBigIntegerConverter.FromString(balanceString);
        var balance = (decimal)balanceInt / WeiPerEther;
        return balance;
    }

    public async Task<decimal[]> GetBalancesAsync(IEnumerable<string> addresses, CancellationToken cancellationToken = default)
    {
        var balanceStrings = await Client.BatchRequestAsync<string>("eth_getBalance", addresses.Select(x => new[] { x, "pending" }), Options, cancellationToken: cancellationToken).ThrowOnError().ConfigureAwait(false);
        return balanceStrings
            .Select(x => EthereumHexBigIntegerConverter.FromString(x.Result))
            .Select(x => (decimal)x / WeiPerEther)
            .ToArray();
    }

    public async Task<EthereumRawTransaction> CreateRawTransactionAsync(string privateKey, string fromAddress, string toAddress, decimal value, BigInteger gasPriceE18, int gasProvided, int nonce, CancellationToken cancellationToken = default)
    {
        // The SHA256 of the private key is the passphrase.
        var passphrase = privateKey.ToHexBytes().ComputeSHA256().ToLowerHex();

        var signParams = new object[]
        {
                new
                {
                    from = fromAddress,
                    to = toAddress,
                    gasPrice = ToHex(gasPriceE18),
                    gas = ToHex(gasProvided),
                    value = ToHex((long)(value * WeiPerEther)),
                    data = "",
                    nonce = ToHex(nonce)
                },
                passphrase
        };

        var tx = await RequestAsync<EthereumRawTransaction>("personal_signTransaction", signParams, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (tx.Ok)
        {
            return tx.Result;
        }

        var import = await RequestAsync<string>("personal_importRawKey", new[] { privateKey, passphrase }, cancellationToken).ConfigureAwait(false);
        if (import.Ok)
        {
            if (import.Result != fromAddress)
            {
                throw new InvalidOperationException($"The address {fromAddress} does not correspond to the one derived from the private key: {import.Result}");
            }
        }
        else if (import.Error!.Code != -32000) // Account not found
        {
            throw new JsonRpcException(import.Error);
        }

        return await RequestAsync<EthereumRawTransaction>("personal_signTransaction", signParams, cancellationToken: cancellationToken).ThrowOnError().ConfigureAwait(false);
    }

    public async Task<int> GetTransactionCountAsync(string address, CancellationToken cancellationToken = default)
    {
        var countString = await RequestAsync<string>("eth_getTransactionCount", new object[] { address, "pending" }, cancellationToken).ThrowOnError().ConfigureAwait(false);
        return EthereumHexInt32Converter.FromString(countString);
    }

    public Task<EthereumLog[]> GetBlockContractEventsAsync(long height, string address, CancellationToken cancellationToken = default) =>
        GetBlockContractEventsAsync(height, height, address, cancellationToken);
    public Task<EthereumLog[]> GetBlockContractEventsAsync(long height, string[] addresses, CancellationToken cancellationToken = default) =>
        GetBlockContractEventsAsync(height, height, addresses, cancellationToken);
    public Task<EthereumLog[]> GetBlockContractEventsAsync(long minHeight, long maxHeight, string address, CancellationToken cancellationToken = default) =>
        GetBlockContractEventsAsync(minHeight, maxHeight, new[] { address }, cancellationToken);
    public async Task<EthereumLog[]> GetBlockContractEventsAsync(long minHeight, long maxHeight, string[] addresses, CancellationToken cancellationToken = default)
    {
        var filterId = await RequestAsync<string>("eth_newFilter", new object[] { new { fromBlock = ToHex(minHeight), toBlock = ToHex(maxHeight), address = addresses, topics = Array.Empty<object>() } }, cancellationToken).ThrowOnError().ConfigureAwait(false);
        var changes = await RequestAsync<EthereumLog[]>("eth_getFilterLogs", new object[] { filterId }, cancellationToken).ThrowOnError().ConfigureAwait(false);
        UninstallFilterAsync(filterId, cancellationToken).SwallowExceptions();
        return changes;
    }
    async Task UninstallFilterAsync(string filterId, CancellationToken cancellationToken = default)
    {
        try
        {
            await RequestValueAsync<bool>("eth_uninstallFilter", new object[] { filterId }, cancellationToken).ThrowOnError().ConfigureAwait(false);
        }
        catch
        {
        }
    }

    public async Task<string> RawCallAsync(string address, string data, CancellationToken cancellationToken = default) => await RequestAsync<string>("eth_call", new object[] { new { to = address, data }, "pending" }, cancellationToken).ThrowOnError().ConfigureAwait(false);

    public async Task<EthereumTransactionReceipt?> GetTransactionReceiptAsync(string hash, CancellationToken cancellationToken = default) =>
        await RequestAsync<EthereumTransactionReceipt>("eth_getTransactionReceipt", new object[] { hash }, cancellationToken).ThrowOrNullOnError().ConfigureAwait(false);
}
