namespace Gateway.Commands;

public abstract class HyperledgerCommand
{
    public virtual string Command => "";
    public virtual string[] GetArgs => [];
}
