namespace HyperledgerSdk.Commands;

public class TransferCommand : HyperledgerCommand
{
    public override string Command => "Transfer";
    public override string[] Args => [FromId, ToId, Amount];
    public string FromId { get; set; }
    public string ToId { get; set; }
    public string Amount { get; set; }
}
