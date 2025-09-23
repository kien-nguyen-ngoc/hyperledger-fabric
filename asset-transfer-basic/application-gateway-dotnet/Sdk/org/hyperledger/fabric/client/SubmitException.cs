/*
 * Copyright 2021 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */


/*
 * Copyright 2021 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Grpc.Core;

namespace org.hyperledger.fabric.client;

/// <summary>
/// Thrown when a failure occurs submitting an endorsed transaction to the orderer.
/// </summary>
public class SubmitException : TransactionException
{
    /// <summary>
    /// Constructs a new exception with the specified cause.
    /// </summary>
    /// <param name="transactionId">a transaction ID.</param>
    /// <param name="cause">the cause.</param>
    public SubmitException(string transactionId, RpcException cause) : base(transactionId, cause)
    {
    }
}
