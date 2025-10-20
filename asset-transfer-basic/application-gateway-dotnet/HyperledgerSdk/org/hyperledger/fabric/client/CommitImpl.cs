using Google.Protobuf;
using Grpc.Core;
using org.hyperledger.fabric.protos.gateway;

namespace org.hyperledger.fabric.client;

public class CommitImpl : ICommit
{
    private readonly GatewayClient client;
    private readonly SigningIdentity signingIdentity;
    private SignedCommitStatusRequest signedRequest;

    internal CommitImpl(
        GatewayClient client,
        SigningIdentity signingIdentity,
        string transactionId,
        SignedCommitStatusRequest signedRequest)
    {
        this.client = client;
        this.signingIdentity = signingIdentity;
        TransactionId = transactionId;
        this.signedRequest = signedRequest;
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

    public string TransactionId { get; }

    public async Task<IStatus> GetStatusAsync(CallOptions? options = null)
    {
        Sign();
        CommitStatusResponse response = await client.CommitStatusAsync(signedRequest, options);
        return new Status(TransactionId, response);
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
