using Grpc.Core;

namespace org.hyperledger.fabric.client;

/// <summary>
/// Options defining runtime behavior of a gRPC service invocation.
/// </summary>
[Obsolete("Use Grpc.Core.CallOptions instead.")]
public sealed class CallOption
{
    private readonly Func<CallOptions, CallOptions> _operator;

    private CallOption(Func<CallOptions, CallOptions> op)
    {
        _operator = op;
    }

    /// <summary>
    /// An absolute deadline.
    /// </summary>
    /// <param name="deadline">the deadline.</param>
    /// <returns>a call option.</returns>
    public static CallOption Deadline(DateTime deadline)
    {
        //ArgumentNullException.ThrowIfNull(deadline, nameof(deadline));
        return new CallOption(options => options.WithDeadline(deadline));
    }

    /// <summary>
    /// A deadline that is after the given duration from when the call is initiated.
    /// </summary>
    /// <param name="duration">a time duration.</param>
    /// <returns>a call option.</returns>
    public static CallOption DeadlineAfter(TimeSpan duration)
    {
        //ArgumentNullException.ThrowIfNull(duration, nameof(duration));
        return new CallOption(options => options.WithDeadline(DateTime.UtcNow.Add(duration)));
    }

    internal CallOptions Apply(CallOptions options)
    {
        return _operator(options);
    }
}
