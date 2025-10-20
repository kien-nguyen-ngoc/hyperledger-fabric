using Google.Protobuf;
using org.hyperledger.fabric.protos.gateway;

namespace org.hyperledger.fabric.client;

public class SubmittedTransaction : CommitImpl, ISubmittedTransaction
{
    internal SubmittedTransaction(
        GatewayClient client,
        SigningIdentity signingIdentity,
        string transactionId,
        SignedCommitStatusRequest signedRequest,
        ByteString result) : base(client, signingIdentity, transactionId, signedRequest)
    {
        ResultBytes = result;
    }

    private ByteString ResultBytes { get; }
    public byte[] Result => ResultBytes.ToByteArray();
}
