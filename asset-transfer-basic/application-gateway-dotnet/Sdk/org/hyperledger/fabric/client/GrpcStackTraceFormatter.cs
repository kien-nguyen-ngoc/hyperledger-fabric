/*
 * Copyright 2021 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using org.hyperledger.fabric.protos.gateway;
using System.Text;

namespace org.hyperledger.fabric.client;

public class GrpcStackTraceFormatter
{
    private readonly string originalMessage;
    private readonly GrpcStatus grpcStatus;

    public GrpcStackTraceFormatter(string originalMessage, GrpcStatus grpcStatus)
    {
        this.originalMessage = originalMessage;
        this.grpcStatus = grpcStatus;
    }

    public string GetFormattedMessage()
    {
        var message = new StringBuilder(originalMessage);

        var details = grpcStatus.Details.ToList();
        if (details.Count > 0)
        {
            message.Append("\nError details:\n");
            foreach (ErrorDetail detail in details)
            {
                message.Append("    address: ")
                    .Append(detail.Address)
                    .Append("; mspId: ")
                    .Append(detail.MspId)
                    .Append("; message: ")
                    .Append(detail.Message)
                    .Append('\n');
            }
        }

        return message.ToString();
    }
}
