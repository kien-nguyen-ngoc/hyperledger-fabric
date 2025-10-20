using Grpc.Core;

namespace org.hyperledger.fabric.client;

/// <summary>
/// A Fabric Gateway call to obtain events.
/// </summary>
/// <typeparam name="T">The type of events obtained by this request.</typeparam>
public interface IEventsRequest<T> : ISignable
{
    /// <summary>
    /// Get events. The gRPC implementation may not begin reading events until the first use of the returned
    /// iterator.
    /// <para>Note that the returned iterator may throw <see cref="GatewayRuntimeException"/> during iteration if a gRPC
    /// connection error occurs.</para>
    /// </summary>
    /// <param name="options">Function that transforms call options.</param>
    /// <returns>Ordered sequence of events.</returns>
    IAsyncEnumerable<T> GetEvents(CallOptions? options = null);
}
