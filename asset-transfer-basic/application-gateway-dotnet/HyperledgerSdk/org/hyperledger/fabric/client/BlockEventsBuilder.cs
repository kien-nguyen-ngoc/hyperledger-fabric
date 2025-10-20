using Google.Protobuf;
using org.hyperledger.fabric.protos.common;

namespace org.hyperledger.fabric.client;

public class BlockEventsBuilder : IBlockEventsRequest.IBuilder
{
    private readonly GatewayClient client;
    private readonly SigningIdentity signingIdentity;
    private readonly BlockEventsEnvelopeBuilder envelopeBuilder;

    internal BlockEventsBuilder(
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

    public IBlockEventsRequest.IBuilder StartBlock(long blockNumber)
    {
        envelopeBuilder.StartBlock(blockNumber);
        return this;
    }

    public IBlockEventsRequest.IBuilder Checkpoint(ICheckpoint checkpoint)
    {
        if (checkpoint.BlockNumber.HasValue)
        {
            envelopeBuilder.StartBlock(checkpoint.BlockNumber.Value);
        }
        return this;
    }

    public IBlockEventsRequest Build()
    {
        Envelope request = envelopeBuilder.Build();
        return new BlockEventsRequest(client, signingIdentity, request);
    }

    IEventsBuilder<IBlockEventsRequest, Block> IEventsBuilder<IBlockEventsRequest, Block>.StartBlock(long blockNumber)
    {
        return StartBlock(blockNumber);
    }

    IEventsBuilder<IBlockEventsRequest, Block> IEventsBuilder<IBlockEventsRequest, Block>.Checkpoint(ICheckpoint checkpoint)
    {
        return Checkpoint(checkpoint);
    }
}
