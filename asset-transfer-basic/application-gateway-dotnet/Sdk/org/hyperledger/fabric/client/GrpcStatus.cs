/*
 * Copyright 2021 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Grpc.Core;
using org.hyperledger.fabric.protos.gateway;

namespace org.hyperledger.fabric.client;

public class GrpcStatus
{
    private readonly Grpc.Core.Status status;
    private readonly Metadata trailers;

    public GrpcStatus(Grpc.Core.Status status, Metadata trailers)
    {
        this.status = status;
        this.trailers = trailers ?? new Metadata();
    }

    public Grpc.Core.Status Status => status;

    public IEnumerable<ErrorDetail> Details 
    { 
        get 
        {
            return trailers.Select(trailer => new ErrorDetail { Message = $"[{status}] {trailer.Key}: {trailer.Value}"});
        } 
    }
}
