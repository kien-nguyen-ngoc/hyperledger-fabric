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

using org.hyperledger.fabric.protos.peer;

namespace org.hyperledger.fabric.client;

/// <summary>
/// Thrown when a transaction fails to commit successfully.
/// </summary>
public class CommitException : Exception
{
    private static string GetMessage(IStatus status)
    {
        TxValidationCode code = status.Code;
        return "Commit of transaction " + status.TransactionId + " failed with status code " + (int)code
               + " (" + code.ToString() + ")";
    }

    /// <summary>
    /// Constructs a new commit exception for the specified transaction.
    /// </summary>
    /// <param name="status">Transaction commit status.</param>
    public CommitException(IStatus status) : base(GetMessage(status))
    {
        Status = status;
    }

    public IStatus Status { get; }

    /// <summary>
    /// Get the ID of the transaction.
    /// </summary>
    /// <returns>transaction ID.</returns>
    public string TransactionId => Status.TransactionId;

    /// <summary>
    /// Get the transaction status code.
    /// </summary>
    /// <returns>transaction status code.</returns>
    public TxValidationCode Code => Status.Code;
}
