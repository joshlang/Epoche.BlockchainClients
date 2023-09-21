using Epoche.BlockchainClients.JsonRpc;

namespace Epoche.BlockchainClients.Harmony;
public class HarmonyClient : IHarmonyClient
{
    static readonly JsonRpcRequestOptions Options = new();

    readonly IJsonRpcClient Client;
    public HarmonyClient(IJsonRpcClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    Task<JsonRpcResult<T>> RequestAsync<T>(string method, object? request, CancellationToken cancellationToken = default) where T : class => Client.RequestAsync<T>(method: method, request: request, requestOptions: Options, cancellationToken: cancellationToken);

    public async Task<HarmonyTransaction[]> GetTransactionsAsync(string account, CancellationToken cancellationToken = default)
    {
        List<HarmonyTransaction> txes = new();
        for (var page = 0; ; ++page)
        {
            var result = await RequestAsync<HarmonyTransactions>("hmy_getTransactionsHistory", new object[] { new { address = "0x6a5dceff750561ae05ba3eafd961ad5fdc73a12b", pageIndex = page, pageSize = 100, fullTx = true } }, cancellationToken).ThrowOrNullOnError();
            txes.AddRange(result!.Transactions);
            if (result!.Transactions.Length < 100) { break; }
        }
        return txes.ToArray();
    }
}
