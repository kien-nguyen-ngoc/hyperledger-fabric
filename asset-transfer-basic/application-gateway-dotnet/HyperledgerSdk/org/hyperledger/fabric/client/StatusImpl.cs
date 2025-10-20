using org.hyperledger.fabric.protos.gateway;
using org.hyperledger.fabric.protos.peer;

namespace org.hyperledger.fabric.client;

/// <summary>
/// Information about a transaction that is committed to the ledger.
/// </summary>
public class Status : IStatus
{
    internal Status(string transactionId, CommitStatusResponse response)
    {
        TransactionId = transactionId;
        BlockNumber = (long)response.BlockNumber;
        Code = response.Result;
    }

    public string TransactionId { get; }
    public long BlockNumber { get; }
    public TxValidationCode Code { get; }
    public bool IsSuccessful => Code == TxValidationCode.Valid;

    public override string ToString()
    {
        return GatewayUtils.ToString(
            this,
            $"transactionId: {TransactionId}",
            $"code: {(int)Code} ({Code})",
            $"blockNumber: {BlockNumber}");
    }
}
