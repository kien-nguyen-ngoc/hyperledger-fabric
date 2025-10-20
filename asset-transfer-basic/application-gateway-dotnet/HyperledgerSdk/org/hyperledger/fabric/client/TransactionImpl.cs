using Google.Protobuf;
using Grpc.Core;
using org.hyperledger.fabric.protos.gateway;

namespace org.hyperledger.fabric.client;

public class Transaction : ITransaction
{
    private readonly GatewayClient client;
    private readonly SigningIdentity signingIdentity;
    private readonly string channelName;
    private PreparedTransaction preparedTransaction;
    private readonly ByteString result;

    internal Transaction(
        GatewayClient client,
        SigningIdentity signingIdentity,
        PreparedTransaction preparedTransaction)
    {
        this.client = client;
        this.signingIdentity = signingIdentity;
        this.preparedTransaction = preparedTransaction;

        var parser = new TransactionEnvelopeParser(preparedTransaction.Envelope);
        channelName = parser.ChannelName;
        result = parser.Result;
    }

    public byte[] Result => result.ToByteArray();

    public byte[] GetBytes()
    {
        return preparedTransaction.ToByteArray();
    }

    public byte[] GetDigest()
    {
        byte[] message = preparedTransaction.Envelope.Payload.ToByteArray();
        return signingIdentity.Hash(message);
    }

    public string TransactionId => preparedTransaction.TransactionId;

    public async Task<byte[]> SubmitAsync(CallOptions? options = null)
    {
        var commit = await SubmitAsyncNoWait(options);
        var status = await commit.GetStatusAsync(options);
        if (!status.IsSuccessful)
        {
            throw new CommitException(status);
        }

        return Result;
    }

    public async Task<ISubmittedTransaction> SubmitAsyncNoWait(CallOptions? options = null)
    {
        Sign();
        var submitRequest = new SubmitRequest
        {
            TransactionId = preparedTransaction.TransactionId,
            ChannelId = channelName,
            PreparedTransaction = preparedTransaction.Envelope
        };
        await client.SubmitAsync(submitRequest, options);

        return new SubmittedTransaction(
            client, signingIdentity, TransactionId, NewSignedCommitStatusRequest(), result);
    }

    public void SetSignature(byte[] signature)
    {
        var envelope = preparedTransaction.Envelope.Clone();
        envelope.Signature = ByteString.CopyFrom(signature);
        preparedTransaction.Envelope = envelope;
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
        return !preparedTransaction.Envelope.Signature.IsEmpty;
    }

    private SignedCommitStatusRequest NewSignedCommitStatusRequest()
    {
        return new SignedCommitStatusRequest
        {
            Request = NewCommitStatusRequest().ToByteString()
        };
    }

    private CommitStatusRequest NewCommitStatusRequest()
    {
        ByteString creator = ByteString.CopyFrom(signingIdentity.Creator);
        return new CommitStatusRequest
        {
            ChannelId = channelName,
            TransactionId = preparedTransaction.TransactionId,
            Identity = creator
        };
    }
}
