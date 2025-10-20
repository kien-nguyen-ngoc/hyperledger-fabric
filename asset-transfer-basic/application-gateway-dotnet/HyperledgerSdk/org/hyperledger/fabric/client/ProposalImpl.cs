using Google.Protobuf;
using Grpc.Core;
using org.hyperledger.fabric.protos.gateway;

namespace org.hyperledger.fabric.client;

public class Proposal : IProposal
{
    private readonly GatewayClient client;
    private readonly SigningIdentity signingIdentity;
    private readonly string channelName;
    private ProposedTransaction proposedTransaction;

    internal Proposal(
        GatewayClient client,
        SigningIdentity signingIdentity,
        string channelName,
        ProposedTransaction proposedTransaction)
    {
        this.client = client;
        this.signingIdentity = signingIdentity;
        this.channelName = channelName;
        this.proposedTransaction = proposedTransaction;
    }

    public string TransactionId => proposedTransaction.TransactionId;

    public byte[] GetBytes()
    {
        return proposedTransaction.ToByteArray();
    }

    public byte[] GetDigest()
    {
        byte[] message = proposedTransaction.Proposal.ProposalBytes.ToByteArray();
        return signingIdentity.Hash(message);
    }

    public async Task<byte[]> EvaluateAsync(CallOptions? options = null)
    {
        Sign();
        var evaluateRequest = new EvaluateRequest
        {
            TransactionId = proposedTransaction.TransactionId,
            ChannelId = channelName,
            ProposedTransaction = proposedTransaction.Proposal,
        };
        evaluateRequest.TargetOrganizations.AddRange(proposedTransaction.EndorsingOrganizations);
        var response = await client.EvaluateAsync(evaluateRequest, options);
        return response.Result.Payload.ToByteArray();
    }

    public async Task<ITransaction> EndorseAsync(CallOptions? options = null)
    {
        Sign();
        var endorseRequest = new EndorseRequest
        {
            TransactionId = proposedTransaction.TransactionId,
            ChannelId = channelName,
            ProposedTransaction = proposedTransaction.Proposal,
        };
        endorseRequest.EndorsingOrganizations.AddRange(proposedTransaction.EndorsingOrganizations);
        EndorseResponse endorseResponse = await client.EndorseAsync(endorseRequest, options);

        var preparedTransaction = new PreparedTransaction
        {
            TransactionId = proposedTransaction.TransactionId,
            Envelope = endorseResponse.PreparedTransaction
        };
        return new Transaction(client, signingIdentity, preparedTransaction);
    }

    public void SetSignature(byte[] signature)
    {
        var signedProposal = proposedTransaction.Proposal.Clone();
        signedProposal.Signature = ByteString.CopyFrom(signature);
        proposedTransaction.Proposal = signedProposal;
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
        return !proposedTransaction.Proposal.Signature.IsEmpty;
    }
}
