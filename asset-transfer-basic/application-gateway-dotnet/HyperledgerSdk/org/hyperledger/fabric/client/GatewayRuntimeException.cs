using Grpc.Core;
using org.hyperledger.fabric.protos.gateway;

namespace org.hyperledger.fabric.client;

/// <summary>
/// Thrown if an error is encountered while invoking gRPC services on a gateway peer. Since the gateway delegates much
/// of the processing to other nodes (endorsing peers and orderers), then the error could have originated from one or
/// more of those nodes. In that case, the details will contain errors information from those nodes.
/// </summary>
public class GatewayRuntimeException : Exception
{
    private readonly GrpcStatus grpcStatus;

    /// <summary>
    /// Constructs a new exception with the specified cause.
    /// </summary>
    /// <param name="cause">the cause.</param>
    public GatewayRuntimeException(RpcException cause) : base(cause.Message, cause)
    {
        grpcStatus = new GrpcStatus(cause.Status, cause.Trailers);
    }

    /// <summary>
    /// Returns the status code as a gRPC Status object.
    /// </summary>
    /// <returns>gRPC call status.</returns>
    public Grpc.Core.Status Status => grpcStatus.Status;

    /// <summary>
    /// Get the gRPC error details returned by a gRPC invocation failure.
    /// </summary>
    /// <returns>error details.</returns>
    public IEnumerable<ErrorDetail> Details => grpcStatus.Details;

    public override string Message => new GrpcStackTraceFormatter(base.Message, grpcStatus).GetFormattedMessage();
}
