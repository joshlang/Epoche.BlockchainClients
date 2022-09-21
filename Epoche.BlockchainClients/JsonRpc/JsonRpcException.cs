namespace Epoche.BlockchainClients.JsonRpc;

public class JsonRpcException : Exception
{
    public readonly JsonRpcError? Error;

    public JsonRpcException(string message) : base(message)
    {
    }
    public JsonRpcException(string message, Exception exception) : base(message, exception)
    {
    }
    public JsonRpcException(JsonRpcError error) : base(error.ToString())
    {
        Error = error;
    }
}
