using org.hyperledger.fabric.protos.peer;

namespace org.hyperledger.fabric.client;

/// <summary>
/// Information about a transaction that is committed to the ledger.
/// </summary>
public interface IStatus
{
    /// <summary>
    /// Get the transaction ID.
    /// </summary>
    /// <returns>A transaction ID.</returns>
    string TransactionId { get; }

    /// <summary>
    /// Get the block number in which the transaction committed.
    /// <para>Note that the block number is an unsigned 64-bit integer, with the sign bit used to hold the top bit of
    /// the number.</para>
    /// </summary>
    /// <returns>A block number.</returns>
    long BlockNumber { get; }

    /// <summary>
    /// Get the committed transaction status code.
    /// </summary>
    /// <returns>Transaction status code.</returns>
    TxValidationCode Code { get; }

    /// <summary>
    /// Check whether the transaction committed successfully.
    /// </summary>
    /// <returns><c>true</c> if the transaction committed successfully; otherwise <c>false</c>.</returns>
    bool IsSuccessful { get; }
}
