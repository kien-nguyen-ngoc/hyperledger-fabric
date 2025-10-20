using System.Security.Cryptography;

namespace org.hyperledger.fabric.client.identity
{
    /// <summary>
    /// A signing implementation used to generate digital signatures. Standard implementations can be obtained using factory
    /// methods on the <see cref="Signers"/> class.
    /// </summary>
    public interface ISigner
    {
        /// <summary>
        /// Signs the supplied message digest. The digest is typically a hash of the message.
        /// </summary>
        /// <param name="digest">A message digest.</param>
        /// <returns>A digital signature.</returns>
        /// <exception cref="CryptographicException">if a signing error occurs.</exception>
        byte[] Sign(byte[] digest);
    }
}
