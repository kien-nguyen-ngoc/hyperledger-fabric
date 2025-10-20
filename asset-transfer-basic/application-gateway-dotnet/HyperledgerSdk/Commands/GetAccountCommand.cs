namespace HyperledgerSdk.Commands;

public class GetAccountCommand : HyperledgerCommand
{
    public override string Command => "ReadAccount";
    public override string[] Args => [Id];
    public string Id { get; set; }
}
