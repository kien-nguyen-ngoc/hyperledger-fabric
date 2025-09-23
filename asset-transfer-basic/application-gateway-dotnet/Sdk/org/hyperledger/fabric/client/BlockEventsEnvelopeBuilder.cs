/*
 * Copyright 2022 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Google.Protobuf;
using org.hyperledger.fabric.protos.common;
using org.hyperledger.fabric.protos.orderer;

namespace org.hyperledger.fabric.client;

public class BlockEventsEnvelopeBuilder
{
    private readonly SigningIdentity signingIdentity;
    private readonly string channelName;
    private readonly ByteString tlsCertificateHash;
    private readonly StartPositionBuilder startPositionBuilder = new();

    internal BlockEventsEnvelopeBuilder(
        SigningIdentity signingIdentity, string channelName, ByteString tlsCertificateHash)
    {
        this.signingIdentity = signingIdentity;
        this.channelName = channelName;
        this.tlsCertificateHash = tlsCertificateHash;
    }

    public BlockEventsEnvelopeBuilder StartBlock(long blockNumber)
    {
        startPositionBuilder.StartBlock(blockNumber);
        return this;
    }

    public Envelope Build()
    {
        return new Envelope { Payload = NewPayload().ToByteString() };
    }

    private Payload NewPayload()
    {
        return new Payload
        {
            Header = NewHeader(),
            Data = NewSeekInfo().ToByteString()
        };
    }

    private Header NewHeader()
    {
        return new Header
        {
            ChannelHeader = NewChannelHeader().ToByteString(),
            SignatureHeader = NewSignatureHeader().ToByteString()
        };
    }

    private ChannelHeader NewChannelHeader()
    {
        return new ChannelHeader
        {
            ChannelId = channelName,
            Epoch = 0,
            Timestamp = GatewayUtils.GetCurrentTimestamp(),
            Type = (int)HeaderType.DeliverSeekInfo,
            TlsCertHash = tlsCertificateHash
        };
    }

    private SignatureHeader NewSignatureHeader()
    {
        ByteString creator = ByteString.CopyFrom(signingIdentity.Creator);
        return new SignatureHeader { Creator = creator };
    }

    private SeekInfo NewSeekInfo()
    {
        return new SeekInfo
        {
            Start = startPositionBuilder.Build(),
            Stop = GatewayUtils.SeekLargestBlockNumber()
        };
    }
}
