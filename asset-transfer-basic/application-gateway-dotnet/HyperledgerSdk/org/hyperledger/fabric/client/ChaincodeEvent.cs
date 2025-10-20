namespace org.hyperledger.fabric.client;

/// <summary>
/// Chaincode event emitted by a transaction function.
/// </summary>
public interface IChaincodeEvent
{
    /// <summary>
    /// Block number that included this chaincode event.
    /// <para>Note that the block number is an unsigned 64-bit integer, with the sign bit used to hold the top bit of
    /// the number.</para>
    /// </summary>
    /// <returns>A block number.</returns>
    long BlockNumber { get; }

    /// <summary>
    /// Transaction that emitted this chaincode event.
    /// </summary>
    /// <returns>Transaction ID.</returns>
    string TransactionId { get; }

    /// <summary>
    /// Chaincode that emitted this event.
    /// </summary>
    /// <returns>Chaincode name.</returns>
    string ChaincodeName { get; }

    /// <summary>
    /// Name of the emitted event.
    /// </summary>
    /// <returns>Event name.</returns>
    string EventName { get; }

    /// <summary>
    /// Application defined payload data associated with this event.
    /// </summary>
    /// <returns>Event payload.</returns>
    byte[] Payload { get; }
}
