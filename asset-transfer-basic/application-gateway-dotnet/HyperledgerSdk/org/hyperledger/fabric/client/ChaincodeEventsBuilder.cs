using Google.Protobuf;
using org.hyperledger.fabric.protos.gateway;

namespace org.hyperledger.fabric.client;

public class ChaincodeEventsBuilder : IChaincodeEventsRequest.IBuilder
{
    private readonly GatewayClient client;
    private readonly SigningIdentity signingIdentity;
    private readonly string channelName;
    private readonly string chaincodeName;
    private readonly StartPositionBuilder startPositionBuilder = new();
    private string afterTransactionId;

    internal ChaincodeEventsBuilder(
        GatewayClient client,
        SigningIdentity signingIdentity,
        string channelName,
        string chaincodeName)
    {
        ArgumentNullException.ThrowIfNull(channelName, nameof(channelName));
        ArgumentNullException.ThrowIfNull(chaincodeName, nameof(chaincodeName));

        this.client = client;
        this.signingIdentity = signingIdentity;
        this.channelName = channelName;
        this.chaincodeName = chaincodeName;
    }

    public IChaincodeEventsRequest.IBuilder StartBlock(long blockNumber)
    {
        startPositionBuilder.StartBlock(blockNumber);
        return this;
    }

    public IChaincodeEventsRequest.IBuilder Checkpoint(ICheckpoint checkpoint)
    {
        if (checkpoint.BlockNumber.HasValue)
            startPositionBuilder.StartBlock(checkpoint.BlockNumber.Value);
        afterTransactionId = checkpoint.TransactionId;
        return this;
    }

    public IChaincodeEventsRequest Build()
    {
        SignedChaincodeEventsRequest signedRequest = NewSignedChaincodeEventsRequestProto();
        return new ChaincodeEventsRequest(client, signingIdentity, signedRequest);
    }

    private SignedChaincodeEventsRequest NewSignedChaincodeEventsRequestProto()
    {
        var request = NewChaincodeEventsRequestProto();
        return new SignedChaincodeEventsRequest
        {
            Request = request.ToByteString()
        };
    }

    private protos.gateway.ChaincodeEventsRequest NewChaincodeEventsRequestProto()
    {
        ByteString creator = ByteString.CopyFrom(signingIdentity.Creator);
        var builder =
            new protos.gateway.ChaincodeEventsRequest
            {
                ChannelId = channelName,
                ChaincodeId = chaincodeName,
                Identity = creator,
                StartPosition = startPositionBuilder.Build()
            };
        if (afterTransactionId != null)
        {
            builder.AfterTransactionId = afterTransactionId;
        }
        return builder;
    }

    IEventsBuilder<IChaincodeEventsRequest, IChaincodeEvent> IEventsBuilder<IChaincodeEventsRequest, IChaincodeEvent>.StartBlock(long blockNumber)
    {
        return StartBlock(blockNumber);
    }

    IEventsBuilder<IChaincodeEventsRequest, IChaincodeEvent> IEventsBuilder<IChaincodeEventsRequest, IChaincodeEvent>.Checkpoint(ICheckpoint checkpoint)
    {
        return Checkpoint(checkpoint);
    }
}
