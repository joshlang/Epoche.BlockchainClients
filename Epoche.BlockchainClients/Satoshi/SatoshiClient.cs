using Epoche.BlockchainClients.JsonRpc;

namespace Epoche.BlockchainClients.Satoshi;

// https://github.com/bitcoin/bitcoin/blob/master/src/rpc/protocol.h
// Error #'s used below are defined by the bitcoin protocol above.

public class SatoshiClient : ISatoshiClient
{
    static readonly JsonRpcRequestOptions Options = new();

    readonly IJsonRpcClient Client;

    public SatoshiClient(IJsonRpcClient jsonRpcClient)
    {
        Client = jsonRpcClient ?? throw new ArgumentNullException(nameof(jsonRpcClient));
    }

    Task<JsonRpcResult<T>> RequestAsync<T>(string method, object? request, CancellationToken cancellationToken = default) where T : class => Client.RequestAsync<T>(method: method, request: request, requestOptions: Options, cancellationToken: cancellationToken);
    Task<JsonRpcResult<T>> RequestValueAsync<T>(string method, object? request, CancellationToken cancellationToken = default) where T : struct => Client.RequestValueAsync<T>(method: method, request: request, requestOptions: Options, cancellationToken: cancellationToken);

    Task<SatoshiBlockSummary?> RequestBlockSummaryAsync(string hash, CancellationToken cancellationToken = default)
    {
        if (hash is null)
        {
            throw new ArgumentNullException(nameof(hash));
        }
        return RequestAsync<SatoshiBlockSummary>("getblock", new[] { hash }, cancellationToken).ThrowOrNullOnError();
    }

    Task<SatoshiTransaction?> RequestTransactionAsync(string hash, CancellationToken cancellationToken = default)
    {
        if (hash is null)
        {
            throw new ArgumentNullException(nameof(hash));
        }
        return RequestAsync<SatoshiTransaction>("getrawtransaction", new object[] { hash, true }, cancellationToken).ThrowOrNullOnError();
    }

    async Task<SatoshiTransaction[]> RequestTransactionsAsync(IEnumerable<string> hashes, bool ignoreMissing, CancellationToken cancellationToken = default)
    {
        if (hashes is null)
        {
            throw new ArgumentNullException(nameof(hashes));
        }
        var transactions = await Client.BatchRequestAsync<SatoshiTransaction>("getrawtransaction", hashes.Select(x => new object[] { x, true }), Options, cancellationToken).ConfigureAwait(false);
        if (!ignoreMissing)
        {
            transactions.FirstError?.Throw();
        }
        return transactions
            .Results
            .Select(x => x.ThrowOrNullOnError())
            .ExcludeNull()
            .ToArray();
    }

    public async Task<string[][]> DecodeScriptAddressesAsync(string[] scripts, CancellationToken cancellationToken = default)
    {
        if (scripts is null)
        {
            throw new ArgumentNullException(nameof(scripts));
        }

        var pks = await Client.BatchRequestAsync<SatoshiScriptPubKey>("decodescript", scripts.Select(x => new object[] { x }), Options, cancellationToken).ConfigureAwait(false);
        return pks
            .Results
            .Select(x => x?.Result?.Addresses ?? Array.Empty<string>())
            .ToArray();
    }

    public async Task<string[]> DecodeScriptAddressesAsync(string script, CancellationToken cancellationToken = default)
    {
        var pks = await RequestAsync<SatoshiScriptPubKey>("decodescript", new object[] { script }, cancellationToken).ThrowOnError().ConfigureAwait(false);
        return pks.Addresses ?? Array.Empty<string>();
    }

    public bool SupportsGetRawTransaction => true;

    public Task<long> GetBlockCountAsync(CancellationToken cancellationToken = default) => RequestValueAsync<long>("getblockcount", null, cancellationToken).ThrowOnError();

    public async Task<BlockInfo> GetBestBlockAsync(CancellationToken cancellationToken = default)
    {
        var blockCount = await GetBlockCountAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        var hash = await RequestAsync<string>("getblockhash", new[] { blockCount }, cancellationToken).ThrowOnError().ConfigureAwait(false);
        var summary = await RequestAsync<SatoshiBlockSummary>("getblock", new[] { hash }, cancellationToken).ThrowOnError().ConfigureAwait(false);
        return new BlockInfo(height: blockCount, hash: hash, summary.PreviousBlockHash);
    }

    public async Task<BlockInfo?> GetBlockAsync(long height, CancellationToken cancellationToken = default)
    {
        var hash = await RequestAsync<string>("getblockhash", new[] { height }, cancellationToken).ThrowOrNullOnError().ConfigureAwait(false);
        if (hash is null)
        {
            return null;
        }

        var summary = await RequestAsync<SatoshiBlockSummary>("getblock", new[] { hash }, cancellationToken).ThrowOnError().ConfigureAwait(false);
        return new BlockInfo(height: height, hash: hash, summary.PreviousBlockHash);
    }

    public async Task<string?> GetBlockHashAsync(long height, CancellationToken cancellationToken = default) =>
        await RequestAsync<string>("getblockhash", new[] { height }, cancellationToken).ThrowOrNullOnError().ConfigureAwait(false);

    public async Task<BlockInfo?> GetBlockAsync(string hash, CancellationToken cancellationToken = default)
    {
        var summary = await RequestBlockSummaryAsync(hash: hash, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (summary is null)
        {
            return null;
        }

        return new BlockInfo(height: summary.Height, hash: hash, summary.PreviousBlockHash);
    }

    public Task<string?> GetRawTransactionAsync(string hash, CancellationToken cancellationToken = default)
    {
        if (hash is null)
        {
            throw new ArgumentNullException(nameof(hash));
        }
        return RequestAsync<string>("getrawtransaction", new object[] { hash, false }, cancellationToken).ThrowOrNullOnError();
    }

    public Task<string?> GetRawBlockAsync(string hash, CancellationToken cancellationToken = default)
    {
        if (hash is null)
        {
            throw new ArgumentNullException(nameof(hash));
        }
        return RequestAsync<string>("getblock", new object[] { hash, false }, cancellationToken).ThrowOrNullOnError();
    }

    public async Task<TransactionInfo[]?> GetBlockTransactionsAsync(string hash, CancellationToken cancellationToken = default)
    {
        var summary = await RequestBlockSummaryAsync(hash: hash, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (summary is null)
        {
            return null;
        }

        return await GetTransactionsAsync(hashes: summary.TransactionHashes, ignoreMissing: false, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<TransactionInfo[]> GetTransactionsAsync(IEnumerable<string> hashes, bool ignoreMissing, CancellationToken cancellationToken = default)
    {
        var transactions = await RequestTransactionsAsync(hashes: hashes, ignoreMissing: ignoreMissing, cancellationToken: cancellationToken).ConfigureAwait(false);

        return transactions
            .Select(x => x.ToTransactionInfo())
            .ToArray();
    }

    public async Task<TransactionInfo?> GetTransactionAsync(string hash, CancellationToken cancellationToken = default)
    {
        var transaction = await RequestTransactionAsync(hash: hash, cancellationToken: cancellationToken).ConfigureAwait(false);
        return transaction?.ToTransactionInfo();
    }

    public async Task<long?> GetTransactionConfirmationsAsync(string hash, CancellationToken cancellationToken = default)
    {
        var transaction = await RequestTransactionAsync(hash: hash, cancellationToken: cancellationToken).ConfigureAwait(false);
        return transaction?.Confirmations;
    }

    public async Task<bool> ValidatePublicAddressAsync(string address, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(address))
        {
            return false;
        }

        var validateResult = await RequestAsync<SatoshiValidateAddress>("validateaddress", new object[] { address }, cancellationToken).ThrowOnError().ConfigureAwait(false);
        return validateResult.IsValid;
    }

    public async Task<bool[]> ValidatePublicAddressesAsync(IEnumerable<string> addresses, CancellationToken cancellationToken = default)
    {
        if (addresses is null)
        {
            throw new ArgumentNullException(nameof(addresses));
        }

        var allAddresses = addresses.ToList();
        var results = new bool[allAddresses.Count];
        if (allAddresses.Count == 0)
        {
            return results;
        }

        var validateResults = await Client.BatchRequestAsync<SatoshiValidateAddress>("validateaddress", allAddresses.Select(address => new object[] { address }), Options, cancellationToken).ThrowOnError().ConfigureAwait(false);
        var validAddresses = validateResults
            .Select(x => x.Result)
            .Where(x => x.IsValid)
            .Select(x => x.Address)
            .ToHashSet();

        for (var x = 0; x < allAddresses.Count; ++x)
        {
            results[x] = validAddresses.Contains(allAddresses[x]);
        }
        return results;
    }

    public async Task<bool> SendRawTransactionAsync(string rawTransaction, CancellationToken cancellationToken = default)
    {
        if (rawTransaction is null)
        {
            throw new ArgumentNullException(nameof(rawTransaction));
        }

        var sendResult = await RequestAsync<string>("sendrawtransaction", new object[] { rawTransaction }, cancellationToken).ConfigureAwait(false);
        if (sendResult.Ok)
        {
            return true;
        }

        return sendResult.Error!.Code switch
        {
            -27 => true, // Already in chain (and confirmed)
            -25 => false, // Unknown input, probably.  Can be non-standard tx.
            -26 => false, // Rejected by network rules
            -8 => false, // Input already spent
            _ => throw new JsonRpcException(sendResult.Error)
        };
    }

    public async Task<SatoshiValidateAddress[]> GetPublicAddressInfoAsync(IEnumerable<string> addresses, CancellationToken cancellationToken = default)
    {
        if (addresses is null)
        {
            throw new ArgumentNullException(nameof(addresses));
        }

        var validateResults = await Client.BatchRequestAsync<SatoshiValidateAddress>("validateaddress", addresses.Select(x => new object[] { x }), Options, cancellationToken).ThrowOnError().ConfigureAwait(false);
        return validateResults.Select(x => x.Result).ToArray();
    }

    public async Task<int> GetTransactionSizeWithoutInputsAsync(IEnumerable<string> outputAddresses, CancellationToken cancellationToken = default)
    {
        if (outputAddresses is null)
        {
            throw new ArgumentNullException(nameof(outputAddresses));
        }

        var createRaw = await RequestAsync<string>("createrawtransaction", new object[] { Array.Empty<object>(), outputAddresses.ToDictionary(x => x, x => "1") }, cancellationToken).ThrowOnError().ConfigureAwait(false);
        return createRaw.Length;
    }

    public Task<SatoshiNetworkInfo> GetNetworkInfoAsync(CancellationToken cancellationToken = default) => RequestAsync<SatoshiNetworkInfo>("getnetworkinfo", null, cancellationToken).ThrowOnError();

    public Task<SatoshiTransaction[]> GetTransactionDetailsAsync(IEnumerable<string> hashes, bool ignoreMissing, CancellationToken cancellationToken = default) => RequestTransactionsAsync(hashes: hashes, ignoreMissing: ignoreMissing, cancellationToken: cancellationToken);

    public Task<string[]> GetMempoolTransactionHashesAsync(CancellationToken cancellationToken = default) => RequestAsync<string[]>("getrawmempool", null, cancellationToken).ThrowOnError();

    public async Task<SatoshiTransactionFeeInfo[]> GetTransactionFeesAsync(IEnumerable<string> hashes, bool ignoreMissing, CancellationToken cancellationToken = default)
    {
        var transactions = await RequestTransactionsAsync(hashes: hashes, ignoreMissing: ignoreMissing, cancellationToken: cancellationToken).ConfigureAwait(false);
        var inputHashes = transactions
            .SelectMany(x => x.Inputs)
            .Select(x => x.Hash)
            .ToHashSet();
        var inputTransactions = await RequestTransactionsAsync(hashes: inputHashes, ignoreMissing: ignoreMissing, cancellationToken: cancellationToken).ConfigureAwait(false);
        var inputTxesByHash = inputTransactions.ToDictionary(x => x.StandardHash);

        return transactions
            .Where(x => x.Inputs.Select(x => x.Hash).All(inputTxesByHash.ContainsKey))
            .Select(x => new
            {
                Transaction = x,
                TotalInput = x.Inputs.Select(i => inputTxesByHash[i.Hash].Outputs[i.Index].Value).Sum(),
                TotalOutput = x.Outputs.Select(o => o.Value).Sum()
            })
            .Select(x => new SatoshiTransactionFeeInfo(satoshiTransaction: x.Transaction, fee: x.TotalInput - x.TotalOutput))
            .ToArray();
    }

    public async Task<int> GetMaxBlockSizeAsync(CancellationToken cancellationToken = default)
    {
        var blockTemplate = await RequestAsync<SatoshiBlockTemplate>("getblocktemplate", new[] { new { rules = new[] { "segwit" } } }, cancellationToken).ThrowOnError().ConfigureAwait(false);
        return blockTemplate.SizeLimit / 4;
    }

    public async Task<SatoshiUnspentInfo?[]> GetUnspentInfoAsync(IEnumerable<(string Hash, int Index)> transactionReferences, CancellationToken cancellationToken = default)
    {
        if (transactionReferences is null)
        {
            throw new ArgumentNullException(nameof(transactionReferences));
        }

        var unspents = await Client.BatchRequestAsync<SatoshiUnspentInfo?>("gettxout", transactionReferences.Select(x => new object[] { x.Hash, x.Index, true }), Options, cancellationToken).ThrowOnError().ConfigureAwait(false);
        return unspents.Select(x => x.Result).ToArray();
    }

    bool? UseSignTransactionWithKey;
    public async Task<SatoshiSignTransactionResult> CreateSignedTransactionAsync(IEnumerable<(string Hash, int Index)> inputs, IEnumerable<(string Address, decimal Value)> outputs, IEnumerable<string> privateKeyAddresses, CancellationToken cancellationToken = default)
    {
        var unsigned = await RequestAsync<string>(
            method: "createrawtransaction",
            cancellationToken: cancellationToken,
            request: new object[]
            {
                    inputs.Select(x => new
                    {
                        txid = x.Hash,
                        vout = x.Index
                    }).ToArray(),
                    outputs.ToDictionary(x => x.Address, x => x.Value.ToString())
            })
            .ThrowOnError()
            .ConfigureAwait(false);
        var privateKeys = privateKeyAddresses.ToArray();
        if (UseSignTransactionWithKey is null || UseSignTransactionWithKey == false)
        {
            var signed = await RequestAsync<SatoshiSignTransactionResult>(
                method: "signrawtransaction",
                cancellationToken: cancellationToken,
                request: new object?[]
                {
                        unsigned,
                        null,
                        privateKeys
                })
                .ConfigureAwait(false);
            if (signed.Ok)
            {
                UseSignTransactionWithKey = false;
                return signed.Result;
            }
            if (signed.Error!.Code != -32 && // returned by newer clients which deprecated it and want "signrawtransactionwithkey" instead.  Litecoin, for example.  Dogecoin still uses signrawtransaction.
                signed.Error!.Code != -32601) // returned by newer clients which removed it entirely.  Bitcoin, for example.
            {
                signed.Error.Throw();
            }
            UseSignTransactionWithKey ??= true;
        }
        return await RequestAsync<SatoshiSignTransactionResult>(
            method: "signrawtransactionwithkey",
            cancellationToken: cancellationToken,
            request: new object?[]
            {
                    unsigned,
                    privateKeys
            })
            .ThrowOnError()
            .ConfigureAwait(false);
    }

    public Task<SatoshiTransaction> DecodeTransactionAsync(string rawTransaction, CancellationToken cancellationToken = default) => RequestAsync<SatoshiTransaction>("decoderawtransaction", new object[] { rawTransaction }, cancellationToken).ThrowOnError();

    public Task ImportPrivateKeyAsync(string privateKey, bool rescan, string label = "", CancellationToken cancellationToken = default) => RequestAsync<object>("importprivkey", new object[] { privateKey, label ?? "", rescan }, cancellationToken).ThrowOnError();
    public Task ImportPrivateKeysAsync(string[] privateKeys, bool rescan, CancellationToken cancellationToken = default) => RequestAsync<object>("importprivatekeys", new object[] { rescan ? "rescan" : "no-rescan" }.Concat(privateKeys).ToArray(), cancellationToken).ThrowOnError();
}