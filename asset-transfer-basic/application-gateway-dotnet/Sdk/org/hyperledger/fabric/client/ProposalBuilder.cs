/*
 * Copyright 2020 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.Text;
using Google.Protobuf;
using org.hyperledger.fabric.protos.common;
using org.hyperledger.fabric.protos.gateway;
using org.hyperledger.fabric.protos.peer;

namespace org.hyperledger.fabric.client;

public class ProposalBuilder : IProposal.IBuilder
{
    private readonly GatewayClient client;
    private readonly SigningIdentity signingIdentity;
    private readonly string channelName;
    private readonly ChaincodeID chaincodeId;
    private readonly ChaincodeInput inputBuilder = new();
    private readonly ChaincodeProposalPayload payloadBuilder = new();
    private ISet<string> endorsingOrgs = new HashSet<string>();

    internal ProposalBuilder(
        GatewayClient client,
        SigningIdentity signingIdentity,
        string channelName,
        string chaincodeName,
        string transactionName)
    {
        this.client = client;
        this.signingIdentity = signingIdentity;
        this.channelName = channelName;
        chaincodeId = new ChaincodeID { Name = chaincodeName };

        inputBuilder.Args.Add(ByteString.CopyFrom(transactionName, Encoding.UTF8));
    }

    public IProposal.IBuilder AddArguments(params byte[][] args)
    {
        foreach (byte[] arg in args)
        {
            inputBuilder.Args.Add(ByteString.CopyFrom(arg));
        }
        return this;
    }

    public IProposal.IBuilder AddArguments(params string[] args)
    {
        foreach (string arg in args)
        {
            inputBuilder.Args.Add(ByteString.CopyFrom(arg, Encoding.UTF8));
        }
        return this;
    }

    public IProposal.IBuilder PutAllTransient(IDictionary<string, byte[]> transientData)
    {
        foreach (var entry in transientData)
        {
            PutTransient(entry.Key, entry.Value);
        }
        return this;
    }

    public IProposal.IBuilder PutTransient(string key, byte[] value)
    {
        payloadBuilder.TransientMap[key] = ByteString.CopyFrom(value);
        return this;
    }

    public IProposal.IBuilder PutTransient(string key, string value)
    {
        payloadBuilder.TransientMap[key] = ByteString.CopyFromUtf8(value);
        return this;
    }

    public IProposal.IBuilder SetEndorsingOrganizations(params string[] mspids)
    {
        endorsingOrgs = new HashSet<string>(mspids);
        return this;
    }

    public IProposal Build()
    {
        return new Proposal(client, signingIdentity, channelName, NewProposedTransaction());
    }

    private ProposedTransaction NewProposedTransaction()
    {
        var context = new TransactionContext(signingIdentity);
        var result = new ProposedTransaction
        {
            Proposal = NewSignedProposal(context),
            TransactionId = context.TransactionId,
        };
        result.EndorsingOrganizations.AddRange(endorsingOrgs);
        return result;
    }

    private SignedProposal NewSignedProposal(TransactionContext context)
    {
        return new SignedProposal
        {
            ProposalBytes = NewProposal(context).ToByteString()
        };
    }

    private protos.peer.Proposal NewProposal(TransactionContext context)
    {
        return new protos.peer.Proposal
        {
            Header = NewHeader(context).ToByteString(),
            Payload = NewChaincodeProposalPayload().ToByteString()
        };
    }

    private Header NewHeader(TransactionContext context)
    {
        return new Header
        {
            ChannelHeader = NewChannelHeader(context).ToByteString(),
            SignatureHeader = context.SignatureHeader.ToByteString()
        };
    }

    private ChannelHeader NewChannelHeader(TransactionContext context)
    {
        return new ChannelHeader
        {
            Type = (int)HeaderType.EndorserTransaction,
            TxId = context.TransactionId,
            Timestamp = GatewayUtils.GetCurrentTimestamp(),
            ChannelId = channelName,
            Extension = NewChaincodeHeaderExtension().ToByteString(),
            Epoch = 0
        };
    }

    private ChaincodeHeaderExtension NewChaincodeHeaderExtension()
    {
        return new ChaincodeHeaderExtension { ChaincodeId = chaincodeId };
    }

    private ChaincodeProposalPayload NewChaincodeProposalPayload()
    {
        payloadBuilder.Input = NewChaincodeInvocationSpec().ToByteString();
        return payloadBuilder;
    }

    private ChaincodeInvocationSpec NewChaincodeInvocationSpec()
    {
        return new ChaincodeInvocationSpec
        {
            ChaincodeSpec = NewChaincodeSpec()
        };
    }

    private ChaincodeSpec NewChaincodeSpec()
    {
        return new ChaincodeSpec
        {
            ChaincodeId = chaincodeId,
            Input = inputBuilder
        };
    }
}
