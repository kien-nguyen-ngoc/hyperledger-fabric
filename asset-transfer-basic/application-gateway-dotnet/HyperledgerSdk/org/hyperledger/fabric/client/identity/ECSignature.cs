using Org.BouncyCastle.Asn1;
using System.Security.Cryptography;

namespace org.hyperledger.fabric.client.identity
{
    public class ECSignature
    {
        public DerInteger R { get; }
        public DerInteger S { get; }

        internal static ECSignature FromBytes(byte[] derSignature)
        {
            using var inStream = new MemoryStream(derSignature);
            using var asnInputStream = new Asn1InputStream(inStream);
            Asn1Object asn1 = asnInputStream.ReadObject();

            if (!(asn1 is Asn1Sequence asn1Sequence))
            {
                throw new CryptographicException("Invalid signature type: " + asn1.GetType().FullName);
            }

            var signatureParts = asn1Sequence.Cast<Asn1Object>()
                .Where(asn1Primitive => asn1Primitive is DerInteger)
                .Cast<DerInteger>()
                .ToList();

            if (signatureParts.Count != 2)
            {
                throw new CryptographicException("Invalid signature. Expected 2 values but got " + signatureParts.Count);
            }

            return new ECSignature(signatureParts[0], signatureParts[1]);
        }

        internal ECSignature(DerInteger r, DerInteger s)
        {
            R = r;
            S = s;
        }

        public byte[] GetBytes()
        {
            using var bytesOut = new MemoryStream();
            var sequence = new DerSequenceGenerator(bytesOut);
            sequence.AddObject(R);
            sequence.AddObject(S);
            sequence.Close();
            return bytesOut.ToArray();
        }
    }
}
