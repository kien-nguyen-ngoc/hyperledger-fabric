namespace Gateway.Commands;

public class CreateAccountCommand : HyperledgerCommand
{
    public override string Command => "CreateAccount";
    public override string[] GetArgs => [Id];
    public string Id { get; set; }
}
