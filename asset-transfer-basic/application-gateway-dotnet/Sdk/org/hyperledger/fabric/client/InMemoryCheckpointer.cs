/*
 * Copyright 2022 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

namespace org.hyperledger.fabric.client;

/// <summary>
/// A non-persistent Checkpointer implementation.
/// It can be used to checkpoint progress after successfully processing events, allowing eventing to be resumed from this point.
/// </summary>
public sealed class InMemoryCheckpointer : ICheckpointer
{
    public long? BlockNumber { get; private set; }
    public string? TransactionId { get; private set; }

    public ValueTask CheckpointBlockAsync(long blockNumber)
    {
        return CheckpointTransactionAsync(blockNumber + 1, null);
    }

    public ValueTask CheckpointTransactionAsync(long blockNumber, string? transactionId)
    {
        BlockNumber = blockNumber;
        TransactionId = transactionId;
        return ValueTask.CompletedTask;
    }

    public ValueTask CheckpointChaincodeEventAsync(IChaincodeEvent @event)
    {
        return CheckpointTransactionAsync(@event.BlockNumber, @event.TransactionId);
    }
}
