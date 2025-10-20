namespace HyperledgerSdk.Commands;

public class DeductBalanceCommand : HyperledgerCommand
{
    public override string Command => "DeductBalance";
    public override string[] Args => [Id, Amount];
    public string Id { get; set; }
    public string Amount { get; set; }
}
