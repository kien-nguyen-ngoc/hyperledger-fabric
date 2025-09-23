/*
 * Copyright 2019 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Grpc.Core;
using org.hyperledger.fabric.client.identity;

namespace org.hyperledger.fabric.client;

public interface IGateway : IDisposable
{
    static IBuilder NewInstance()
    {
        return new Gateway.Builder();
    }

    IIdentity Identity { get; }
    INetwork GetNetwork(string networkName);

    IProposal NewSignedProposal(byte[] proposalBytes, byte[] signature);
    IProposal NewProposal(byte[] proposalBytes);

    ITransaction NewSignedTransaction(byte[] transactionBytes, byte[] signature);
    ITransaction NewTransaction(byte[] transactionBytes);

    ICommit NewSignedCommit(byte[] bytes, byte[] signature);
    ICommit NewCommit(byte[] bytes);

    IChaincodeEventsRequest NewSignedChaincodeEventsRequest(byte[] bytes, byte[] signature);
    IChaincodeEventsRequest NewChaincodeEventsRequest(byte[] bytes);

    IBlockEventsRequest NewSignedBlockEventsRequest(byte[] bytes, byte[] signature);
    IBlockEventsRequest NewBlockEventsRequest(byte[] bytes);

    IFilteredBlockEventsRequest NewSignedFilteredBlockEventsRequest(byte[] bytes, byte[] signature);
    IFilteredBlockEventsRequest NewFilteredBlockEventsRequest(byte[] bytes);

    IBlockAndPrivateDataEventsRequest NewSignedBlockAndPrivateDataEventsRequest(byte[] bytes, byte[] signature);
    IBlockAndPrivateDataEventsRequest NewBlockAndPrivateDataEventsRequest(byte[] bytes);

    public interface IBuilder
    {
        IBuilder Connection(ChannelBase grpcChannel);
        IBuilder Identity(IIdentity identity);
        IBuilder Signer(ISigner signer);
        IBuilder Hash(Func<byte[], byte[]> hash);
        IBuilder TlsClientCertificateHash(byte[] certificateHash);
        IBuilder EvaluateOptions(Func<CallOptions, CallOptions> options);
        IBuilder EndorseOptions(Func<CallOptions, CallOptions> options);
        IBuilder SubmitOptions(Func<CallOptions, CallOptions> options);
        IBuilder CommitStatusOptions(Func<CallOptions, CallOptions> options);
        IBuilder ChaincodeEventsOptions(Func<CallOptions, CallOptions> options);
        IBuilder BlockEventsOptions(Func<CallOptions, CallOptions> options);
        IBuilder FilteredBlockEventsOptions(Func<CallOptions, CallOptions> options);
        IBuilder BlockAndPrivateDataEventsOptions(Func<CallOptions, CallOptions> options);
        IGateway Connect();
    }
}
