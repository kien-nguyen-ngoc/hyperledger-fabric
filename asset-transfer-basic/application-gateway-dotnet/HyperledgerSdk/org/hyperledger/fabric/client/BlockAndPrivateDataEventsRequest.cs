using org.hyperledger.fabric.protos.peer;

namespace org.hyperledger.fabric.client;

/// <summary>
/// A Fabric Gateway call to obtain block and private data events. Supports off-line signing flow using
/// <see cref="Gateway.NewSignedBlockAndPrivateDataEventsRequest(byte[], byte[])"/>.
/// </summary>
public interface IBlockAndPrivateDataEventsRequest : IEventsRequest<BlockAndPrivateData>
{
    /// <summary>
    /// Builder used to create a new block and private data events request. The default behavior is to read events from
    /// the next committed block.
    /// </summary>
    public interface IBuilder : IEventsBuilder<IBlockAndPrivateDataEventsRequest, BlockAndPrivateData>
    {
        new IBuilder StartBlock(long blockNumber);

        new IBuilder Checkpoint(ICheckpoint checkpoint);
    }
}