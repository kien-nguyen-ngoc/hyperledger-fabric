using Grpc.Core;

namespace org.hyperledger.fabric.client;

/// <summary>
/// Thrown when a failure occurs obtaining the commit status of a transaction.
/// </summary>
public class CommitStatusException : TransactionException
{
    /// <summary>
    /// Constructs a new exception with the specified cause.
    /// </summary>
    /// <param name="transactionId">a transaction ID.</param>
    /// <param name="cause">the cause.</param>
    public CommitStatusException(string transactionId, RpcException cause) : base(transactionId, cause)
    {
    }
}
