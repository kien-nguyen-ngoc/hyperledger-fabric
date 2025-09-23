/*
 * Copyright 2022 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Grpc.Core;
using org.hyperledger.fabric.protos.common;
using org.hyperledger.fabric.protos.peer;

namespace org.hyperledger.fabric.client;

/// <summary>
/// A Fabric Gateway call to obtain filtered block events. Supports off-line signing flow using
/// <see cref="IGateway.NewSignedFilteredBlockEventsRequest(byte[], byte[])"/>.
/// </summary>
public interface IFilteredBlockEventsRequest : IEventsRequest<FilteredBlock>
{
    /// <summary>
    /// Builder used to create a new filtered block events request. The default behavior is to read events from the next
    /// committed block.
    /// </summary>
    public interface IBuilder : IEventsBuilder<IFilteredBlockEventsRequest, FilteredBlock>
    {
        new IBuilder StartBlock(long blockNumber);
        new IBuilder Checkpoint(ICheckpoint checkpoint);
    }
}

public class FilteredBlockEventsRequest : SignableBlockEventsRequest, IFilteredBlockEventsRequest
{
    private readonly GatewayClient client;

    internal FilteredBlockEventsRequest(
        GatewayClient client, SigningIdentity signingIdentity, Envelope request) : base(signingIdentity, request)
    {
        this.client = client;
    }

    public IAsyncEnumerable<FilteredBlock> GetEvents(CallOptions? options = null)
    {
        Envelope signedRequest = GetSignedRequest();
        IAsyncEnumerable<DeliverResponse> responseIter = client.FilteredBlockEvents(signedRequest, options);

        return responseIter.Select(response =>
        {
            if (response.TypeCase == DeliverResponse.TypeOneofCase.Status)
            {
                throw new Exception($"Unexpected status response: {response.Status}");
            }

            return response.FilteredBlock;
        });
    }
}
