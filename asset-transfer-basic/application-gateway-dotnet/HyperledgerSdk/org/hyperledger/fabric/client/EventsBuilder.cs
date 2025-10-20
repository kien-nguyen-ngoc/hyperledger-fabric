namespace org.hyperledger.fabric.client;

/// <summary>
/// Builder used to create a new events request.
///
/// <para>If both a start block and checkpoint are specified, and the checkpoint has a valid position set, the
/// checkpoint position is used and the specified start block is ignored. If the checkpoint is unset then the start block
/// is used.</para>
///
/// <para>If no start position is specified, eventing begins from the next committed block.</para>
/// </summary>
/// <typeparam name="TRequest">Event request type returned by the builder.</typeparam>
/// <typeparam name="TEvent">Event type returned by the request.</typeparam>
public interface IEventsBuilder<out TRequest, TEvent> : IBuilder<TRequest> where TRequest : IEventsRequest<TEvent>
{
    /// <summary>
    /// Specify the block number at which to start reading events.
    /// <para>Note that the block number is an unsigned 64-bit integer, with the sign bit used to hold the top bit of
    /// the number.</para>
    /// </summary>
    /// <param name="blockNumber">a ledger block number.</param>
    /// <returns>This builder.</returns>
    IEventsBuilder<TRequest, TEvent> StartBlock(long blockNumber);

    /// <summary>
    /// Reads events starting at the checkpoint position.
    /// </summary>
    /// <param name="checkpoint">a checkpoint position.</param>
    /// <returns>This builder.</returns>
    IEventsBuilder<TRequest, TEvent> Checkpoint(ICheckpoint checkpoint);
}
