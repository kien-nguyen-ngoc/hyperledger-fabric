namespace org.hyperledger.fabric.client;

/// <summary>
/// A builder used to create new object instances from configuration state.
/// </summary>
/// <typeparam name="T">The type of object built.</typeparam>
public interface IBuilder<out T>
{
    /// <summary>
    /// Build an instance.
    /// </summary>
    /// <returns>A built instance.</returns>
    T Build();
}
