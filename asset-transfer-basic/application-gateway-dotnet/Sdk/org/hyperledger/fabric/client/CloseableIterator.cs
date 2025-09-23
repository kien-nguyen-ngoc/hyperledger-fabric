/*
 * Copyright 2021 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

namespace org.hyperledger.fabric.client;

/// <summary>
/// An iterator that can be closed when the consumer does not want to read any more elements, freeing up resources that
/// may be held by the iterator.
/// <para>Note that iteration may throw <see cref="GatewayRuntimeException"/> if the gRPC connection fails.</para>
/// </summary>
/// <typeparam name="T">The type of elements returned by this iterator.</typeparam>
public interface ICloseableAsyncEnumerator<out T> : IAsyncEnumerator<T>, IAsyncDisposable
{
}
