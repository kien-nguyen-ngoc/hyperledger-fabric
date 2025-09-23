/*
 * Copyright 2020 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */


/*
 * Copyright 2020 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Grpc.Core;

namespace org.hyperledger.fabric.client;

/// <summary>
/// An endorsed transaction that can be submitted to the orderer for commit to the ledger.
/// </summary>
public interface ITransaction : ISignable
{
    /// <summary>
    /// Get the transaction result. The result is obtained as part of the proposal endorsement so may be read
    /// immediately. It is not necessary to submit the transaction before getting the transaction result, but the
    /// transaction will not be committed to the ledger and its effects visible to other clients and transactions until
    /// after it has been submitted to the orderer.
    /// </summary>
    /// <returns>A transaction result.</returns>
    byte[] Result { get; }

    /// <summary>
    /// Get the transaction ID.
    /// </summary>
    /// <returns>A transaction ID.</returns>
    string TransactionId { get; }

    /// <summary>
    /// Submit the transaction to the orderer to be committed to the ledger. This method blocks until the transaction
    /// has been successfully committed to the ledger.
    /// </summary>
    /// <param name="options">Function that transforms call options.</param>
    /// <returns>A transaction result.</returns>
    /// <exception cref="SubmitException">if the submit invocation fails.</exception>
    /// <exception cref="CommitStatusException">if the commit status invocation fails.</exception>
    /// <exception cref="CommitException">if the transaction commits unsuccessfully.</exception>
    Task<byte[]> SubmitAsync(CallOptions? options = null);

    /// <summary>
    /// Submit the transaction to the orderer to be committed to the ledger. This method returns immediately after the
    /// transaction is successfully delivered to the orderer. The returned Commit may be used to subsequently wait
    /// for the transaction to be committed to the ledger.
    /// </summary>
    /// <param name="options">Function that transforms call options.</param>
    /// <returns>A transaction commit.</returns>
    /// <exception cref="SubmitException">if the gRPC service invocation fails.</exception>
    Task<ISubmittedTransaction> SubmitAsyncNoWait(CallOptions? options = null);
}
