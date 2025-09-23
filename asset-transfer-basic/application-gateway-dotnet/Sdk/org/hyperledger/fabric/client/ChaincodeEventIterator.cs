/*
 * Copyright 2021 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using org.hyperledger.fabric.protos.gateway;

namespace org.hyperledger.fabric.client;

public class ChaincodeEventAsyncEnumerable : IAsyncEnumerable<IChaincodeEvent>
{
    private readonly IAsyncEnumerable<ChaincodeEventsResponse> responseStream;

    public ChaincodeEventAsyncEnumerable(IAsyncEnumerable<ChaincodeEventsResponse> responseStream)
    {
        this.responseStream = responseStream;
    }

    public async IAsyncEnumerator<IChaincodeEvent> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        await foreach (var response in responseStream.WithCancellation(cancellationToken))
        {
            foreach (var e in response.Events)
            {
                yield return new ChaincodeEventImpl((long)response.BlockNumber, e);
            }
        }
    }
}
