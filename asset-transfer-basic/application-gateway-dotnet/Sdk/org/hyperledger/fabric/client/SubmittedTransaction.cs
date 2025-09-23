/*
 * Copyright 2021 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

namespace org.hyperledger.fabric.client;

/// <summary>
/// Allows access to the transaction result and its commit status on the ledger.
/// </summary>
public interface ISubmittedTransaction : ICommit
{
    /// <summary>
    /// Get the transaction result. This is obtained during the endorsement process when the transaction proposal is
    /// run on endorsing peers and so is available immediately. The transaction might subsequently fail to commit
    /// successfully.
    /// </summary>
    /// <returns>Transaction result.</returns>
    byte[] Result { get; }
}
