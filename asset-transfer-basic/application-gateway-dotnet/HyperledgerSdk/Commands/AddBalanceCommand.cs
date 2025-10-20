namespace HyperledgerSdk.Commands;

public class AddBalanceCommand : HyperledgerCommand
{
    public override string Command => "AddBalance";
    public override string[] Args => [Id, Amount];
    public string Id { get; set; }
    public string Amount { get; set; }
}
