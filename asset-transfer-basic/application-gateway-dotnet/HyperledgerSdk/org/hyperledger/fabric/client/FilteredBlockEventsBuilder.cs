using Google.Protobuf;
using org.hyperledger.fabric.protos.common;
using org.hyperledger.fabric.protos.peer;

namespace org.hyperledger.fabric.client;

public class FilteredBlockEventsBuilder : IFilteredBlockEventsRequest.IBuilder
{
    private readonly GatewayClient client;
    private readonly SigningIdentity signingIdentity;
    private readonly BlockEventsEnvelopeBuilder envelopeBuilder;

    internal FilteredBlockEventsBuilder(
        GatewayClient client,
        SigningIdentity signingIdentity,
        string channelName,
        ByteString tlsCertificateHash)
    {
        ArgumentNullException.ThrowIfNull(channelName, nameof(channelName));

        this.client = client;
        this.signingIdentity = signingIdentity;
        envelopeBuilder = new BlockEventsEnvelopeBuilder(signingIdentity, channelName, tlsCertificateHash);
    }

    public IFilteredBlockEventsRequest.IBuilder StartBlock(long blockNumber)
    {
        envelopeBuilder.StartBlock(blockNumber);
        return this;
    }

    public IFilteredBlockEventsRequest.IBuilder Checkpoint(ICheckpoint checkpoint)
    {
        if (checkpoint.BlockNumber.HasValue)
        {
            envelopeBuilder.StartBlock(checkpoint.BlockNumber.Value);
        }
        return this;
    }

    public IFilteredBlockEventsRequest Build()
    {
        Envelope request = envelopeBuilder.Build();
        return new FilteredBlockEventsRequest(client, signingIdentity, request);
    }

    IEventsBuilder<IFilteredBlockEventsRequest, FilteredBlock> IEventsBuilder<IFilteredBlockEventsRequest, FilteredBlock>.StartBlock(long blockNumber)
    {
        return StartBlock(blockNumber);
    }

    IEventsBuilder<IFilteredBlockEventsRequest, FilteredBlock> IEventsBuilder<IFilteredBlockEventsRequest, FilteredBlock>.Checkpoint(ICheckpoint checkpoint)
    {
        return Checkpoint(checkpoint);
    }
}
