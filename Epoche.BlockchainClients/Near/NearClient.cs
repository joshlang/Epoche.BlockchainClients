using Epoche.BlockchainClients.JsonRpc;

namespace Epoche.BlockchainClients.Near;
public class NearClient : INearClient
{
    static readonly JsonRpcRequestOptions Options = new();

    readonly IJsonRpcClient Client;
    public NearClient(IJsonRpcClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    Task<JsonRpcResult<T>> RequestAsync<T>(string method, object? request, CancellationToken cancellationToken = default) where T : class => Client.RequestAsync<T>(method: method, request: request, requestOptions: Options, cancellationToken: cancellationToken);

    public async Task<long> GetBlockCountAsync(CancellationToken cancellationToken = default)
    {
        var result = await RequestAsync<NearBlockResult>("block", new { finality = "final" }, cancellationToken).ThrowOrNullOnError();
        return result!.Header.Height;
    }

    static NearBlockResult? ToResult(JsonRpcResult<NearBlockResult> result)
    {
        if (result.Ok) { return result.Result; }
        if (result.Error?.Data?.RootElement.ToString().Contains("DB Not Found Error") != true) { result.ThrowOnError(); }
        return null;
    }

    public async Task<NearBlockResult?> GetBlockAsync(long height, CancellationToken cancellationToken = default) =>
        ToResult(await RequestAsync<NearBlockResult>("block", new { block_id = height }, cancellationToken))!;

    public async Task<NearBlockResult?> GetBlockAsync(string hash, CancellationToken cancellationToken = default) =>
        ToResult(await RequestAsync<NearBlockResult>("block", new { block_id = hash }, cancellationToken))!;

    public async Task<NearTransactionResult?> GetTransactionAsync(string hash, CancellationToken cancellationToken = default)
    {
        var tx = await RequestAsync<NearTransactionResult>("tx", new[] { hash, "sender.mainnet" }, cancellationToken)!;
        if (tx.Ok)
        {
            return tx.Result;
        }
        if (tx.Error?.Data?.RootElement.ToString().Contains(" doesn't exist") != true) { tx.ThrowOnError(); }
        return null;
    }
}
