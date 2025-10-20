using Grpc.Core;

namespace org.hyperledger.fabric.client;

/// <summary>
/// Allows access to information about a transaction that is committed to the ledger.
/// </summary>
public interface ICommit : ISignable
{
    /// <summary>
    /// Get the transaction ID.
    /// </summary>
    /// <returns>A transaction ID.</returns>
    string TransactionId { get; }

    /// <summary>
    /// Get the status of the committed transaction. If the transaction has not yet committed, this method blocks until
    /// the commit occurs.
    /// </summary>
    /// <param name="options">Function that transforms call options.</param>
    /// <returns>Transaction commit status.</returns>
    /// <exception cref="CommitStatusException">if the gRPC service invocation fails.</exception>
    Task<IStatus> GetStatusAsync(CallOptions? options = null);
}
