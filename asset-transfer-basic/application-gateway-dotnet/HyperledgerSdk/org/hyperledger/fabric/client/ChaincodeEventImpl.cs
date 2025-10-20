using Google.Protobuf;

namespace org.hyperledger.fabric.client;

public class ChaincodeEventImpl : IChaincodeEvent
{
    private readonly int hash;

    internal ChaincodeEventImpl(long blockNumber, protos.peer.ChaincodeEvent e)
    {
        BlockNumber = blockNumber;
        TransactionId = e.TxId;
        ChaincodeName = e.ChaincodeId;
        EventName = e.EventName;
        PayloadBytes = e.Payload;
        hash = HashCode.Combine(blockNumber, TransactionId, ChaincodeName, EventName); // Ignore potentially large payload; this is good enough
    }

    public long BlockNumber { get; }
    public string TransactionId { get; }
    public string ChaincodeName { get; }
    public string EventName { get; }
    private ByteString PayloadBytes { get; }
    public byte[] Payload => PayloadBytes.ToByteArray();

    public override bool Equals(object obj)
    {
        if (obj is not ChaincodeEventImpl that)
            return false;

        return BlockNumber == that.BlockNumber &&
               TransactionId == that.TransactionId &&
               ChaincodeName == that.ChaincodeName &&
               EventName == that.EventName &&
               PayloadBytes.Equals(that.PayloadBytes);
    }

    public override int GetHashCode()
    {
        return hash;
    }

    public override string ToString()
    {
        return GatewayUtils.ToString(this,
            $"blockNumber: {BlockNumber}",
            $"transactionId: {TransactionId}",
            $"chaincodeName: {ChaincodeName}",
            $"eventName: {EventName}",
            $"payload: {PayloadBytes}");
    }
}
