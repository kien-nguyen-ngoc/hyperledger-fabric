namespace HyperledgerSdk.Commands;

public class CreateAccountCommand : HyperledgerCommand
{
    public override string Command => "CreateAccount";
    public override string[] Args => [Id];
    public string Id { get; set; }
}
