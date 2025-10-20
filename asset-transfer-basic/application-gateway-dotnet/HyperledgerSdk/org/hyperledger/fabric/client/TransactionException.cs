using Grpc.Core;

namespace org.hyperledger.fabric.client;

/// <summary>
/// Thrown when a failure occurs invoking a transaction.
/// </summary>
public class TransactionException : GatewayException
{
    /// <summary>
    /// Constructs a new exception with the specified cause.
    /// </summary>
    /// <param name="transactionId">a transaction ID.</param>
    /// <param name="cause">the cause.</param>
    public TransactionException(string transactionId, RpcException cause) : base(cause)
    {
        TransactionId = transactionId;
    }

    /// <summary>
    /// The ID of the transaction.
    /// </summary>
    /// <returns>a transaction ID.</returns>
    public string TransactionId { get; }
}
