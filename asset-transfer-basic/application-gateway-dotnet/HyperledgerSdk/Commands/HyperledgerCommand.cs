namespace HyperledgerSdk.Commands;

public abstract class HyperledgerCommand
{
    public virtual string Command => "";
    public virtual string[] Args => [];
}
