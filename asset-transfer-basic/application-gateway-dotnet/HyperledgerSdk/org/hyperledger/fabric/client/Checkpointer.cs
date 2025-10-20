namespace org.hyperledger.fabric.client;

/// <summary>
/// Checkpointer allows update of a checkpoint position after events are successfully processed.
/// </summary>
public interface ICheckpointer : ICheckpoint
{
    /// <summary>
    /// Checkpoint a successfully processed block.
    /// <para>Note that the block number is an unsigned 64-bit integer, with the sign bit used to hold the top bit of
    /// the number.</para>
    /// </summary>
    /// <param name="blockNumber">a ledger block number.</param>
    /// <exception cref="IOException">if an I/O error occurs.</exception>
    ValueTask CheckpointBlockAsync(long blockNumber);

    /// <summary>
    /// Checkpoint a transaction within a block.
    /// </summary>
    /// <param name="blockNumber">a ledger block number.</param>
    /// <param name="transactionId">transaction id within the block.</param>
    /// <exception cref="IOException">if an I/O error occurs.</exception>
    ValueTask CheckpointTransactionAsync(long blockNumber, string transactionId);

    /// <summary>
    /// Checkpoint a chaincode event.
    /// </summary>
    /// <param name="event">a chaincode event.</param>
    /// <exception cref="IOException">if an I/O error occurs.</exception>
    ValueTask CheckpointChaincodeEventAsync(IChaincodeEvent @event);
}
