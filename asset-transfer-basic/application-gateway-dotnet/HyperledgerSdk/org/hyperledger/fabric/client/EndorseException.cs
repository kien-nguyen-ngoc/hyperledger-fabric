using Grpc.Core;

namespace org.hyperledger.fabric.client;

/// <summary>
/// Thrown when a failure occurs endorsing a transaction proposal.
/// </summary>
public class EndorseException : TransactionException
{
    /// <summary>
    /// Constructs a new exception with the specified cause.
    /// </summary>
    /// <param name="transactionId">a transaction ID.</param>
    /// <param name="cause">the cause.</param>
    public EndorseException(string transactionId, RpcException cause) : base(transactionId, cause)
    {
    }
}
