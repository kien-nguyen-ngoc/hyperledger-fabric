using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace org.hyperledger.fabric.client.identity
{
    /// <summary>
    /// Utility methods for creating and manipulating identity information.
    /// </summary>
    public static class Identities
    {
        /// <summary>
        /// Read a PEM format X.509 certificate.
        /// </summary>
        /// <param name="pem">PEM data.</param>
        /// <returns>An X.509 certificate.</returns>
        public static X509Certificate2 ReadX509Certificate(string pem)
        {
            return ReadX509Certificate(new StringReader(pem));
        }

        /// <summary>
        /// Read a PEM format X.509 certificate.
        /// </summary>
        /// <param name="pemReader">Reader of PEM data.</param>
        /// <returns>An X.509 certificate.</returns>
        public static X509Certificate2 ReadX509Certificate(TextReader pemReader)
        {
            var pemObject = ReadPemObject(pemReader);
            if (pemObject is X509Certificate certificate)
                return new X509Certificate2(certificate);

            throw new CryptographicException("Unexpected PEM content type: " + pemObject.GetType().Name);
        }

        private static object ReadPemObject(TextReader reader)
        {
            var pemReader = new PemReader(reader);
            var result = pemReader.ReadObject();
            if (result == null)
            {
                throw new PemException("Invalid PEM content");
            }
            return result;
        }

        /// <summary>
        /// Read a PEM format private key.
        /// </summary>
        /// <param name="pem">PEM data.</param>
        /// <returns>A private key.</returns>
        public static AsymmetricKeyParameter ReadPrivateKey(string pem)
        {
            return ReadPrivateKey(new StringReader(pem));
        }

        /// <summary>
        /// Read a PEM format private key.
        /// </summary>
        /// <param name="pemReader">Reader of PEM data.</param>
        /// <returns>A private key.</returns>
        public static AsymmetricKeyParameter ReadPrivateKey(TextReader pemReader)
        {
            var pemObject = ReadPemObject(pemReader);
            if (pemObject is AsymmetricCipherKeyPair keyPair)
                return keyPair.Private;
            if (pemObject is AsymmetricKeyParameter privateKey)
                return privateKey;

            throw new InvalidKeyException("Unexpected PEM content type: " + pemObject.GetType().Name);
        }

        /// <summary>
        /// Converts the argument to a PEM format string.
        /// </summary>
        /// <param name="certificate">A certificate.</param>
        /// <returns>A PEM format string.</returns>
        public static string ToPemString(X509Certificate2 certificate)
        {
            return AsPemString(DotNetUtilities.FromX509Certificate(certificate));
        }

        private static string AsPemString(object obj)
        {
            var stringWriter = new StringWriter();
            var pemWriter = new PemWriter(stringWriter);
            pemWriter.WriteObject(obj);
            pemWriter.Writer.Flush();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Converts the argument to a PKCS #8 PEM format string.
        /// </summary>
        /// <param name="privateKey">A private key.</param>
        /// <returns>A PEM format string.</returns>
        public static string ToPemString(AsymmetricKeyParameter privateKey)
        {
            PrivateKeyInfo pkInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);
            return AsPemString(pkInfo);
        }
    }
}
