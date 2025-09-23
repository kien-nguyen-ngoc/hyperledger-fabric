/*
 * Copyright 2020 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace org.hyperledger.fabric.client.identity
{
    /// <summary>
    /// Factory methods to create standard signing implementations.
    /// </summary>
    public static class Signers
    {
        private const string ED25519_ALGORITHM = "Ed25519";

        /// <summary>
        /// Create a new signer that uses the supplied private key for signing. The <see cref="Identities"/> class provides static
        //* methods to create a <c>PrivateKey</c> object from PEM-format data.
        /// <para>Currently supported private key types are:</para>
        /// <ul>
        ///     <li>ECDSA.</li>
        ///     <li>Ed25519.</li>
        /// </ul>
        /// <para>Note that the Sign implementations have different expectations on the input data supplied to them.</para>
        /// <para>The ECDSA signers operate on a pre-computed message digest, and should be combined with an appropriate hash
        /// algorithm. P-256 is typically used with a SHA-256 hash, and P-384 is typically used with a SHA-384 hash.</para>
        /// <para>The Ed25519 signer operates on the full message content, and should be combined with a
        /// <see cref="Hash.NONE"/> (or no-op) hash implementation to ensure the complete
        /// message is passed to the signer.</para>
        /// </summary>
        /// <param name="privateKey">A private key.</param>
        /// <returns>A signer implementation.</returns>
        public static ISigner NewPrivateKeySigner(AsymmetricKeyParameter privateKey)
        {
            if (privateKey is ECPrivateKeyParameters ecPrivateKey)
            {
                return new ECPrivateKeySigner(ecPrivateKey);
            }

            if (privateKey is Ed25519PrivateKeyParameters)
            {
                return new PrivateKeySigner(privateKey, ED25519_ALGORITHM);
            }

            throw new ArgumentException("Unsupported private key type: " + privateKey.GetType().Name);
        }
    }
}
