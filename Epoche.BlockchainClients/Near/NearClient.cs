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
    Task<JsonRpcResult<T>> RequestValueAsync<T>(string method, object? request, CancellationToken cancellationToken = default) where T : struct => Client.RequestValueAsync<T>(method: method, request: request, requestOptions: Options, cancellationToken: cancellationToken);

    public async Task<long> GetBlockCountAsync(CancellationToken cancellationToken = default)
    {
        var result = await RequestAsync<NearBlockResult>("block", new { finality = "final" }, cancellationToken).ThrowOrNullOnError();
        return result!.Header.Height;
    }

    public Task<NearBlockResult> GetBlockAsync(long height, CancellationToken cancellationToken = default) =>
        RequestAsync<NearBlockResult>("block", new { block_id = height }, cancellationToken).ThrowOrNullOnError()!;

    public Task<NearBlockResult> GetBlockAsync(string hash, CancellationToken cancellationToken = default) =>
        RequestAsync<NearBlockResult>("block", new { block_id = hash }, cancellationToken).ThrowOrNullOnError()!;

    public Task<NearTransactionResult> GetTransactionAsync(string hash, CancellationToken cancellationToken = default) =>
        RequestAsync<NearTransactionResult>("tx", new[] { hash, "sender.mainnet" }, cancellationToken).ThrowOrNullOnError()!;
}
