using Google.Protobuf;
using Grpc.Core;
using org.hyperledger.fabric.protos.gateway;

namespace org.hyperledger.fabric.client;

/// <summary>
/// A Fabric Gateway call to obtain chaincode events. Supports off-line signing flow using
/// <see cref="IGateway.NewSignedChaincodeEventsRequest(byte[], byte[])"/>.
/// </summary>
public interface IChaincodeEventsRequest : IEventsRequest<IChaincodeEvent>
{
    /// <summary>
    /// Builder used to create a new chaincode events request. The default behavior is to read events from the next
    /// committed block.
    /// </summary>
    public interface IBuilder : IEventsBuilder<IChaincodeEventsRequest, IChaincodeEvent>
    {
        new IBuilder StartBlock(long blockNumber);
        new IBuilder Checkpoint(ICheckpoint checkpoint);
    }
}

public class ChaincodeEventsRequest : IChaincodeEventsRequest
{
    private readonly GatewayClient client;
    private readonly SigningIdentity signingIdentity;
    private SignedChaincodeEventsRequest signedRequest;

    internal ChaincodeEventsRequest(
        GatewayClient client,
        SigningIdentity signingIdentity,
        SignedChaincodeEventsRequest signedRequest)
    {
        this.client = client;
        this.signingIdentity = signingIdentity;
        this.signedRequest = signedRequest;
    }

    public IAsyncEnumerable<IChaincodeEvent> GetEvents(CallOptions? options = null)
    {
        Sign();
        var responseIter = client.ChaincodeEvents(signedRequest, options);
        return new ChaincodeEventAsyncEnumerable(responseIter);
    }

    public byte[] GetBytes()
    {
        return signedRequest.ToByteArray();
    }

    public byte[] GetDigest()
    {
        byte[] message = signedRequest.Request.ToByteArray();
        return signingIdentity.Hash(message);
    }

    public void SetSignature(byte[] signature)
    {
        signedRequest.Signature = ByteString.CopyFrom(signature);
    }

    private void Sign()
    {
        if (IsSigned())
        {
            return;
        }

        byte[] digest = GetDigest();
        byte[] signature = signingIdentity.Sign(digest);
        SetSignature(signature);
    }

    private bool IsSigned()
    {
        return !signedRequest.Signature.IsEmpty;
    }
}
