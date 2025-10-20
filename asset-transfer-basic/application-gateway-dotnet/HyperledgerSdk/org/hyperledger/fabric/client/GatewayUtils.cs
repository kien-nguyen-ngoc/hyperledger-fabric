using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using org.hyperledger.fabric.client.identity;
using org.hyperledger.fabric.protos.msp;
using org.hyperledger.fabric.protos.orderer;
using System.Runtime.CompilerServices;

namespace org.hyperledger.fabric.client;

/// <summary>
/// Utility functions.
/// </summary>
internal static class GatewayUtils
{
    public static string ToString(object o, params string[] additionalInfo)
    {
        var s = $"{o.GetType().Name}@{RuntimeHelpers.GetHashCode(o):x}";
        if (additionalInfo.Length > 0)
            s += $"({string.Join(", ", additionalInfo)})";
        return s;
    }

    public static byte[] Concat(params byte[][] arrays)
    {
        var result = new byte[arrays.Sum(a => a.Length)];
        var offset = 0;
        foreach (var arr in arrays)
        {
            Buffer.BlockCopy(arr, 0, result, offset, arr.Length);
            offset += arr.Length;
        }
        return result;
    }

    public static SerializedIdentity NewSerializedIdentity(IIdentity identity)
    {
        return new SerializedIdentity
        {
            Mspid = identity.MspId,
            IdBytes = ByteString.CopyFrom(identity.Credentials)
        };
    }

    public static Timestamp GetCurrentTimestamp()
    {
        return Timestamp.FromDateTime(DateTime.UtcNow);
    }

    public static SeekPosition SeekLargestBlockNumber()
    {
        var largestBlockNumber = new SeekSpecified { Number = long.MaxValue };
        return new SeekPosition { Specified = largestBlockNumber };
    }

    [Obsolete]
    internal static Func<CallOptions, CallOptions> AsCallOptions(params CallOption[] legacyOptions)
    {
        if (legacyOptions.Length == 0)
        {
            return null;
        }

        return callOptions =>
        {
            foreach (var op in legacyOptions)
            {
                callOptions = op.Apply(callOptions);
            }
            return callOptions;
        };
    }
}
