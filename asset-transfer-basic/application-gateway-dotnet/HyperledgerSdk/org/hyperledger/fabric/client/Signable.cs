namespace org.hyperledger.fabric.client;

/// <summary>
/// A Fabric Gateway call that can be explicitly signed. Supports off-line signing flow.
/// </summary>
public interface ISignable
{
    /// <summary>
    /// Get the serialized message bytes.
    /// Serialized bytes can be used to recreate the object using methods on <see cref="IGateway"/>.
    /// </summary>
    /// <returns>A serialized signable object.</returns>
    byte[] GetBytes();

    /// <summary>
    /// Get the digest of the signable object. This is used to generate a digital signature.
    /// </summary>
    /// <returns>A hash of the signable object.</returns>
    byte[] GetDigest();

    /// <summary>
    /// Sets the signature on the signable object.
    /// </summary>
    /// <param name="signature">The signature.</param>
    void SetSignature(byte[] signature);
}
