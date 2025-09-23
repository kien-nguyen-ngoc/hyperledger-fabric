/*
 * Copyright 2020 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Grpc.Core;
using org.hyperledger.fabric.protos.common;
using org.hyperledger.fabric.protos.peer;

namespace org.hyperledger.fabric.client;

public interface INetwork
{
    IContract GetContract(string chaincodeName);
    IContract GetContract(string chaincodeName, string contractName);
    string Name { get; }

    IAsyncEnumerable<IChaincodeEvent> GetChaincodeEvents(string chaincodeName, CallOptions? options = null);
    IChaincodeEventsRequest.IBuilder NewChaincodeEventsRequest(string chaincodeName);

    IAsyncEnumerable<Block> GetBlockEvents(CallOptions? options = null);
    IBlockEventsRequest.IBuilder NewBlockEventsRequest();

    IAsyncEnumerable<FilteredBlock> GetFilteredBlockEvents(CallOptions? options = null);
    IFilteredBlockEventsRequest.IBuilder NewFilteredBlockEventsRequest();

    IAsyncEnumerable<BlockAndPrivateData> GetBlockAndPrivateDataEvents(CallOptions? options = null);
    IBlockAndPrivateDataEventsRequest.IBuilder NewBlockAndPrivateDataEventsRequest();
}
