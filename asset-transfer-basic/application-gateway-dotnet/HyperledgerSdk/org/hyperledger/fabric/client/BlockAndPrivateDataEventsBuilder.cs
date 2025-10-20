using Google.Protobuf;
using org.hyperledger.fabric.protos.common;
using org.hyperledger.fabric.protos.peer;

namespace org.hyperledger.fabric.client;

public class BlockAndPrivateDataEventsBuilder : IBlockAndPrivateDataEventsRequest.IBuilder
{
    private readonly GatewayClient client;
    private readonly SigningIdentity signingIdentity;
    private readonly BlockEventsEnvelopeBuilder envelopeBuilder;

    internal BlockAndPrivateDataEventsBuilder(
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

    public IBlockAndPrivateDataEventsRequest.IBuilder StartBlock(long blockNumber)
    {
        envelopeBuilder.StartBlock(blockNumber);
        return this;
    }

    public IBlockAndPrivateDataEventsRequest.IBuilder Checkpoint(ICheckpoint checkpoint)
    {
        if (checkpoint.BlockNumber.HasValue)
        {
            envelopeBuilder.StartBlock(checkpoint.BlockNumber.Value);
        }
        return this;
    }

    public IBlockAndPrivateDataEventsRequest Build()
    {
        Envelope request = envelopeBuilder.Build();
        return new BlockAndPrivateDataEventsRequestImpl(client, signingIdentity, request);
    }

    IEventsBuilder<IBlockAndPrivateDataEventsRequest, BlockAndPrivateData> IEventsBuilder<IBlockAndPrivateDataEventsRequest, BlockAndPrivateData>.StartBlock(long blockNumber)
    {
        return StartBlock(blockNumber);
    }

    IEventsBuilder<IBlockAndPrivateDataEventsRequest, BlockAndPrivateData> IEventsBuilder<IBlockAndPrivateDataEventsRequest, BlockAndPrivateData>.Checkpoint(ICheckpoint checkpoint)
    {
        return Checkpoint(checkpoint);
    }
}
