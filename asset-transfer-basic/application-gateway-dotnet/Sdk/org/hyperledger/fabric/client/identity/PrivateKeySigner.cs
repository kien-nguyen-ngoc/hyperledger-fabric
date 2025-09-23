/*
 * Copyright 2023 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Signers;

namespace org.hyperledger.fabric.client.identity
{
    public class PrivateKeySigner : ISigner
    {
        private readonly AsymmetricKeyParameter privateKey;
        private readonly string algorithm;

        internal PrivateKeySigner(AsymmetricKeyParameter privateKey, string algorithm)
        {
            this.privateKey = privateKey;
            this.algorithm = algorithm;
        }

        public byte[] Sign(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            switch (algorithm.ToUpperInvariant())
            {
                case "NONEWITHECDSA":
                    {
                        var signer = new ECDsaSigner();
                        signer.Init(true, privateKey);
                        var rs = signer.GenerateSignature(data);
                        return DerEncode(rs[0], rs[1]);
                    }

                case "ED25519":
                    {
                        var signer = new Ed25519Signer();
                        signer.Init(true, privateKey);
                        signer.BlockUpdate(data, 0, data.Length);
                        return signer.GenerateSignature();
                    }

                default:
                    throw new NotSupportedException($"Algorithm '{algorithm}' is not supported.");
            }
        }

        private static byte[] DerEncode(Org.BouncyCastle.Math.BigInteger r, Org.BouncyCastle.Math.BigInteger s)
        {
            var v = new Org.BouncyCastle.Asn1.DerSequence(
                new Org.BouncyCastle.Asn1.DerInteger(r),
                new Org.BouncyCastle.Asn1.DerInteger(s));

            return v.GetDerEncoded();
        }

    }
}
