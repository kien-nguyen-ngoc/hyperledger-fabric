namespace HyperledgerSdk.Commands;


public class GetTransactionById : HyperledgerCommand
{
    public override string Command => "GetTransactionByID";
    public override string[] Args => [ChannelName, TransactionId];
    public string ChannelName { get; set; }
    public string TransactionId { get; set; }
}

