/*
 * Copyright 2022 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

namespace org.hyperledger.fabric.client;

/// <summary>
/// Checkpoint provides the current position for event processing.
/// </summary>
public interface ICheckpoint
{
    /// <summary>
    /// The block number in which the next event is expected.
    /// </summary>
    /// <returns>A ledger block number.</returns>
    long? BlockNumber { get; }

    /// <summary>
    /// Transaction Id of the last successfully processed event within the current block.
    /// </summary>
    /// <returns>A transaction Id.</returns>
    string? TransactionId { get; }
}
