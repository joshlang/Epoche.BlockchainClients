namespace Epoche.BlockchainClients.JsonRpc;

public abstract class JsonRpcClientBase
{
    static int nextId = RandomHelper.GetRandomPositiveInt32();
    protected static int GetNextId(int consumeCount = 1) => Interlocked.Add(ref nextId, consumeCount);
    protected static readonly JsonRpcRequestOptions DefaultOptions = new();


    protected abstract Task<Stream> GetResponseAsync(byte[] utf8Json, int id, CancellationToken cancellationToken = default);

    protected virtual Task<Stream> GetResponseAsync(string method, object? request, JsonRpcRequestOptions requestOptions, CancellationToken cancellationToken = default)
    {
        if (method is null)
        {
            throw new ArgumentNullException(nameof(method));
        }
        if (requestOptions is null)
        {
            throw new ArgumentNullException(nameof(requestOptions));
        }
        cancellationToken.ThrowIfCancellationRequested();
        if (request is null && !requestOptions.UseVersion2)
        {
            throw new InvalidOperationException($"Request cannot be null with version 1 json rpc requests");
        }
        if (request?.GetType().IsClass == false)
        {
            throw new InvalidOperationException($"Request must be an array or object if provided");
        }

        var id = GetNextId();
        var obj = new Dictionary<string, object>
        {
            ["id"] = id,
            ["method"] = method
        };
        if (request != null)
        {
            obj["params"] = request;
        }
        if (requestOptions.UseVersion2)
        {
            obj["jsonrpc"] = "2.0";
        }
        var serialized = JsonSerializer.SerializeToUtf8Bytes(obj, options: requestOptions.SerializerOptions);

        return GetResponseAsync(utf8Json: serialized, id: id, cancellationToken: cancellationToken);
    }

    public virtual async Task<JsonRpcResult<T>> RequestAsync<T>(string method, object? request, JsonRpcRequestOptions? requestOptions, CancellationToken cancellationToken = default) where T : class
    {
        requestOptions ??= DefaultOptions;
        using var response = await GetResponseAsync(method: method, request: request, requestOptions: requestOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
        //using var ms = new MemoryStream();
        //await response.CopyToAsync(ms);
        //var str = UTF8Encoding.UTF8.GetString(ms.ToArray());
        var rawResult = await JsonSerializer.DeserializeAsync<RawJsonRpcResult<T>>(utf8Json: response, options: requestOptions?.SerializerOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
        return rawResult!.ToRpcResult();
    }

    public virtual async Task<JsonRpcResult<T>> RequestValueAsync<T>(string method, object? request, JsonRpcRequestOptions? requestOptions, CancellationToken cancellationToken = default) where T : struct
    {
        requestOptions ??= DefaultOptions;
        using var response = await GetResponseAsync(method: method, request: request, requestOptions: requestOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
        var rawResult = await JsonSerializer.DeserializeAsync<RawJsonRpcResult<T?>>(utf8Json: response, options: requestOptions?.SerializerOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
        return
            rawResult!.Error != null
            ? new JsonRpcResult<T>(rawResult.Error)
            : rawResult.Result.HasValue
            ? new JsonRpcResult<T>(rawResult.Result.GetValueOrDefault())
            : throw new JsonRpcException("A null result was received for a non-nullable request");
    }

    public virtual async Task<JsonRpcResult<JsonElement>> RequestAsync(string method, object? request, JsonRpcRequestOptions? requestOptions, CancellationToken cancellationToken = default)
    {
        requestOptions ??= DefaultOptions;
        using var response = await GetResponseAsync(method: method, request: request, requestOptions: requestOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
        var rawResult = await JsonDocument.ParseAsync(utf8Json: response, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (rawResult.RootElement.TryGetProperty("error", out var e) && e.ValueKind != JsonValueKind.Null)
        {
            var rawText = e.GetRawText();
            rawResult.Dispose();
            return new JsonRpcResult<JsonElement>(JsonSerializer.Deserialize<JsonRpcError>(rawText) ?? throw new JsonRpcException("A null result was received for a non-nullable request"));
        }
        return new JsonRpcResult<JsonElement>(rawResult.RootElement.GetProperty("result"));
    }
}
