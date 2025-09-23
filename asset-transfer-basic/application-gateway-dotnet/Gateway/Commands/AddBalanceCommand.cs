namespace Gateway.Commands;

public class AddBalanceCommand : HyperledgerCommand
{
    public override string Command => "AddBalance";
    public override string[] GetArgs => [Id, Amount];
    public string Id { get; set; }
    public string Amount { get; set; }
}
