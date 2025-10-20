using Google.Protobuf;
using Grpc.Core;
using org.hyperledger.fabric.client.identity;
using org.hyperledger.fabric.protos.common;
using org.hyperledger.fabric.protos.gateway;

namespace org.hyperledger.fabric.client;

public class Gateway : IGateway
{
    private readonly GatewayClient client;
    private readonly SigningIdentity signingIdentity;
    private readonly ByteString tlsCertificateHash;

    public class Builder : IGateway.IBuilder
    {
        private static readonly ISigner undefinedSigner = new UndefinedSigner();
        public ChannelBase grpcChannel;
        public IIdentity identity;
        public ISigner signer = undefinedSigner;
        public Func<byte[], byte[]> hash = fabric.client.Hash.Sha256;
        public ByteString tlsCertificateHash = ByteString.Empty;
        public readonly DefaultCallOptions.Builder optionsBuilder = DefaultCallOptions.NewBuilder();

        public IGateway.IBuilder Connection(ChannelBase grpcChannel)
        {
            this.grpcChannel = grpcChannel ?? throw new ArgumentNullException(nameof(grpcChannel));
            return this;
        }

        public IGateway.IBuilder Identity(IIdentity identity)
        {
            this.identity = identity ?? throw new ArgumentNullException(nameof(identity));
            return this;
        }

        public IGateway.IBuilder Signer(ISigner signer)
        {
            this.signer = signer ?? throw new ArgumentNullException(nameof(signer));
            return this;
        }

        public IGateway.IBuilder Hash(Func<byte[], byte[]> hash)
        {
            this.hash = hash ?? throw new ArgumentNullException(nameof(hash));
            return this;
        }

        public IGateway.IBuilder TlsClientCertificateHash(byte[] certificateHash)
        {
            tlsCertificateHash = ByteString.CopyFrom(certificateHash ?? throw new ArgumentNullException(nameof(certificateHash)));
            return this;
        }

        public IGateway.IBuilder EvaluateOptions(Func<CallOptions, CallOptions> options)
        {
            optionsBuilder.WithEvaluate(options ?? throw new ArgumentNullException(nameof(options)));
            return this;
        }

        public IGateway.IBuilder EndorseOptions(Func<CallOptions, CallOptions> options)
        {
            optionsBuilder.WithEndorse(options ?? throw new ArgumentNullException(nameof(options)));
            return this;
        }

        public IGateway.IBuilder SubmitOptions(Func<CallOptions, CallOptions> options)
        {
            optionsBuilder.WithSubmit(options ?? throw new ArgumentNullException(nameof(options)));
            return this;
        }

        public IGateway.IBuilder CommitStatusOptions(Func<CallOptions, CallOptions> options)
        {
            optionsBuilder.WithCommitStatus(options ?? throw new ArgumentNullException(nameof(options)));
            return this;
        }

        public IGateway.IBuilder ChaincodeEventsOptions(Func<CallOptions, CallOptions> options)
        {
            optionsBuilder.WithChaincodeEvents(options ?? throw new ArgumentNullException(nameof(options)));
            return this;
        }

        public IGateway.IBuilder BlockEventsOptions(Func<CallOptions, CallOptions> options)
        {
            optionsBuilder.WithBlockEvents(options ?? throw new ArgumentNullException(nameof(options)));
            return this;
        }

        public IGateway.IBuilder FilteredBlockEventsOptions(Func<CallOptions, CallOptions> options)
        {
            optionsBuilder.WithFilteredBlockEvents(options ?? throw new ArgumentNullException(nameof(options)));
            return this;
        }

        public IGateway.IBuilder BlockAndPrivateDataEventsOptions(Func<CallOptions, CallOptions> options)
        {
            optionsBuilder.WithBlockAndPrivateDataEvents(options ?? throw new ArgumentNullException(nameof(options)));
            return this;
        }

        public IGateway Connect()
        {
            return new Gateway(this);
        }

        private class UndefinedSigner : ISigner
        {
            public byte[] Sign(byte[] digest)
            {
                throw new NotSupportedException("No signing implementation supplied");
            }
        }
    }

    private Gateway(Builder builder)
    {
        if (builder.identity == null)
            throw new ArgumentNullException(nameof(builder.identity), "Identity has not been set");
        if (builder.grpcChannel == null)
            throw new ArgumentNullException(nameof(builder.grpcChannel), "gRPC channel has not been set");

        signingIdentity = new SigningIdentity(builder.identity, builder.hash, builder.signer);
        client = new GatewayClient(builder.grpcChannel, builder.optionsBuilder.Build());
        tlsCertificateHash = builder.tlsCertificateHash;
    }

    public IIdentity Identity => signingIdentity.Identity;

    public void Dispose()
    {
        // Nothing to do for now
    }

    public INetwork GetNetwork(string networkName)
    {
        return new Network(client, signingIdentity, networkName, tlsCertificateHash);
    }

    public IProposal NewSignedProposal(byte[] bytes, byte[] signature)
    {
        var result = (Proposal)NewProposal(bytes);
        result.SetSignature(signature);
        return result;
    }

    public IProposal NewProposal(byte[] bytes)
    {
        try
        {
            var proposedTransaction = ProposedTransaction.Parser.ParseFrom(bytes);
            var proposal = protos.peer.Proposal.Parser.ParseFrom(proposedTransaction.Proposal.ProposalBytes);
            var header = Header.Parser.ParseFrom(proposal.Header);
            var channelHeader = ChannelHeader.Parser.ParseFrom(header.ChannelHeader);

            return new Proposal(client, signingIdentity, channelHeader.ChannelId, proposedTransaction);
        }
        catch (InvalidProtocolBufferException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }

    public ITransaction NewSignedTransaction(byte[] bytes, byte[] signature)
    {
        var transaction = (Transaction)NewTransaction(bytes);
        transaction.SetSignature(signature);
        return transaction;
    }

    public ITransaction NewTransaction(byte[] bytes)
    {
        try
        {
            var preparedTransaction = PreparedTransaction.Parser.ParseFrom(bytes);
            return new Transaction(client, signingIdentity, preparedTransaction);
        }
        catch (InvalidProtocolBufferException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }

    public ICommit NewSignedCommit(byte[] bytes, byte[] signature)
    {
        var commit = (CommitImpl)NewCommit(bytes);
        commit.SetSignature(signature);
        return commit;
    }

    public ICommit NewCommit(byte[] bytes)
    {
        try
        {
            var signedRequest = SignedCommitStatusRequest.Parser.ParseFrom(bytes);
            var request = CommitStatusRequest.Parser.ParseFrom(signedRequest.Request);

            return new CommitImpl(client, signingIdentity, request.TransactionId, signedRequest);
        }
        catch (InvalidProtocolBufferException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }

    public IChaincodeEventsRequest NewSignedChaincodeEventsRequest(byte[] bytes, byte[] signature)
    {
        var result = (ChaincodeEventsRequest)NewChaincodeEventsRequest(bytes);
        result.SetSignature(signature);
        return result;
    }

    public IChaincodeEventsRequest NewChaincodeEventsRequest(byte[] bytes)
    {
        try
        {
            var signedRequest = SignedChaincodeEventsRequest.Parser.ParseFrom(bytes);
            return new ChaincodeEventsRequest(client, signingIdentity, signedRequest);
        }
        catch (InvalidProtocolBufferException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }

    public IBlockEventsRequest NewSignedBlockEventsRequest(byte[] bytes, byte[] signature)
    {
        var result = (BlockEventsRequest)NewBlockEventsRequest(bytes);
        result.SetSignature(signature);
        return result;
    }

    public IBlockEventsRequest NewBlockEventsRequest(byte[] bytes)
    {
        try
        {
            var request = Envelope.Parser.ParseFrom(bytes);
            return new BlockEventsRequest(client, signingIdentity, request);
        }
        catch (InvalidProtocolBufferException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }

    public IFilteredBlockEventsRequest NewSignedFilteredBlockEventsRequest(byte[] bytes, byte[] signature)
    {
        var result = (FilteredBlockEventsRequest)NewFilteredBlockEventsRequest(bytes);
        result.SetSignature(signature);
        return result;
    }

    public IFilteredBlockEventsRequest NewFilteredBlockEventsRequest(byte[] bytes)
    {
        try
        {
            var request = Envelope.Parser.ParseFrom(bytes);
            return new FilteredBlockEventsRequest(client, signingIdentity, request);
        }
        catch (InvalidProtocolBufferException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }

    public IBlockAndPrivateDataEventsRequest NewSignedBlockAndPrivateDataEventsRequest(byte[] bytes, byte[] signature)
    {
        var result = (BlockAndPrivateDataEventsRequestImpl)NewBlockAndPrivateDataEventsRequest(bytes);
        result.SetSignature(signature);
        return result;
    }

    public IBlockAndPrivateDataEventsRequest NewBlockAndPrivateDataEventsRequest(byte[] bytes)
    {
        try
        {
            var request = Envelope.Parser.ParseFrom(bytes);
            return new BlockAndPrivateDataEventsRequestImpl(client, signingIdentity, request);
        }
        catch (InvalidProtocolBufferException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }
}
