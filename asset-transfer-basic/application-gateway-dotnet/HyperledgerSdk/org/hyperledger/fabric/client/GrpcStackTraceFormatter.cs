namespace org.hyperledger.fabric.client;

public class GrpcStackTraceFormatter
{
    private readonly string originalMessage;
    private readonly GrpcStatus grpcStatus;

    public GrpcStackTraceFormatter(string originalMessage, GrpcStatus grpcStatus)
    {
        this.originalMessage = originalMessage;
        this.grpcStatus = grpcStatus;
    }

    public string GetFormattedMessage()
    {
        return grpcStatus?.ToString();
    }
}
