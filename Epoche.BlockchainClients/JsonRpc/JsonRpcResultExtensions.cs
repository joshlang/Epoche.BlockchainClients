namespace Epoche.BlockchainClients.JsonRpc;

static class JsonRpcResultExtensions
{
    /// <summary>
    /// For success, we return the result.
    /// If there are any errors at all, we throw JsonRpcException.
    /// </summary>
    public static async Task<T> ThrowOnError<T>(this Task<JsonRpcResult<T>> resultTask) => (await resultTask.ConfigureAwait(false)).ThrowOnError();

    /// <summary>
    /// For success, we return the result.
    /// If there are any errors at all, we throw JsonRpcException.
    /// </summary>
    public static T ThrowOnError<T>(this JsonRpcResult<T> result)
    {
        result.Error?.Throw();
        return result.Result;
    }

    /// <summary>
    /// For success, we return the result.
    /// If there's an error that seems to indicate something's missing (eg: you asked for a transaction but the hash wasn't found), we return null.
    /// For other errors (eg: can't connect), we throw JsonRpcException.
    /// </summary>
    public static async Task<T?> ThrowOrNullOnError<T>(this Task<JsonRpcResult<T>> resultTask) where T : class => (await resultTask.ConfigureAwait(false)).ThrowOrNullOnError();

    /// <summary>
    /// For success, we return the result.
    /// If there's an error that seems to indicate something's missing (eg: you asked for a transaction but the hash wasn't found), we return null.
    /// For other errors (eg: can't connect), we throw JsonRpcException.
    /// </summary>
    public static T? ThrowOrNullOnError<T>(this JsonRpcResult<T> result) where T : class
    {
        if (result.Error != null)
        {
            //https://github.com/bitcoin/bitcoin/blob/master/src/rpc/protocol.h
            if (result.Error.Code == -1 || // block # too big, for example
                result.Error.Code == -5 || // block not found, for example
                result.Error.Code == -8) // hash is incorrect length, for example
            {
                // We return null because the user probably asked for a something that doesn't exist
                return default;
            }

            // Otherwise we throw
            result.Error.Throw();
        }
        return result.Result;
    }
}
