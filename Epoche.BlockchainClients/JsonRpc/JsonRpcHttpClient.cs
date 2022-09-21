using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Epoche.BlockchainClients.JsonRpc;

public class JsonRpcHttpClient : JsonRpcClientBase, IJsonRpcClient
{
    readonly HttpClient HttpClient;
    readonly MediaTypeHeaderValue JsonTypeHeader = new("application/json") { CharSet = "utf-8" };

    public JsonRpcHttpClient(string endpoint, string? username, string? password)
    {
        HttpClient = new HttpClient
        {
            BaseAddress = new Uri(endpoint ?? throw new ArgumentNullException(nameof(endpoint)))
        };
        if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
        }
    }

    protected override Task<Stream> GetResponseAsync(byte[] utf8Json, int id, CancellationToken cancellationToken = default) =>
        GetResponseAsync(utf8Json: utf8Json, cancellationToken: cancellationToken);

    async Task<Stream> GetResponseAsync(byte[] utf8Json, CancellationToken cancellationToken = default)
    {
        var message = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            Content = new ByteArrayContent(utf8Json ?? throw new ArgumentNullException(nameof(utf8Json)))
        };
        message.Content.Headers.ContentType = JsonTypeHeader;
        var response = await HttpClient.SendAsync(
            request: message,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode &&
            response.StatusCode != HttpStatusCode.NotFound &&
            response.StatusCode != HttpStatusCode.BadRequest &&
            response.StatusCode != HttpStatusCode.InternalServerError)
        {
            // Clients seem to follow: https://www.jsonrpc.org/historical/json-rpc-over-http.html (see errors section)
            response.EnsureSuccessStatusCode();
        }
        return await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<JsonRpcBatchResult<T>> BatchRequestAsync<T>(string method, IEnumerable<object> requests, JsonRpcRequestOptions? requestOptions, CancellationToken cancellationToken = default)
    {
        if (method is null)
        {
            throw new ArgumentNullException(nameof(method));
        }
        if (requests is null)
        {
            throw new ArgumentNullException(nameof(requests));
        }

        requestOptions ??= DefaultOptions;
        cancellationToken.ThrowIfCancellationRequested();
        if (!requestOptions.UseVersion2)
        {
            throw new InvalidOperationException($"Batch requests require version 2 of the json rpc");
        }

        var allRequests = requests.ToList();
        if (allRequests.Count == 0)
        {
            return new JsonRpcBatchResult<T>(Array.Empty<JsonRpcResult<T>>());
        }

        var lastId = GetNextId(allRequests.Count);
        var firstId = lastId - allRequests.Count + 1;
        var nextId = firstId;

        var obj = allRequests
            .Select(x => new
            {
                id = nextId++,
                method,
                @params = x,
                jsonrpc = "2.0"
            })
            .ToArray();
        var rawResultsInOrder = new JsonRpcResult<T>[allRequests.Count];
        foreach (var segment in obj.ReuseableSegment(requestOptions.MaxRequestsPerBatch))
        {
            var serialized = JsonSerializer.SerializeToUtf8Bytes(segment, options: requestOptions!.SerializerOptions);
            using var response = await GetResponseAsync(utf8Json: serialized, cancellationToken: cancellationToken).ConfigureAwait(false);
            var rawResults = await JsonSerializer.DeserializeAsync<RawJsonRpcResult<T>[]>(utf8Json: response, options: requestOptions?.SerializerOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
            foreach (var result in rawResults!)
            {
                if (result.Id < firstId || result.Id > lastId)
                {
                    throw new JsonRpcException("Invalid json rpc batch id received");
                }
                else if (rawResultsInOrder[result.Id - firstId] != null)
                {
                    throw new JsonRpcException("Duplicate json rpc batch id received");
                }
                else
                {
                    rawResultsInOrder[result.Id - firstId] = result.ToRpcResult();
                }
            }
        }
        return new JsonRpcBatchResult<T>(rawResultsInOrder);
    }
}
