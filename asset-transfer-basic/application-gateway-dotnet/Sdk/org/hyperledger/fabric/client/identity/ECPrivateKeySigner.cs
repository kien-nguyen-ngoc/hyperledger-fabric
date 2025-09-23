/*
 * Copyright 2020 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace org.hyperledger.fabric.client.identity
{
    public class ECPrivateKeySigner : ISigner
    {
        private const string ALGORITHM_NAME = "NONEwithECDSA";

        private readonly ISigner signer;
        private readonly BigInteger curveN;
        private readonly BigInteger halfCurveN;

        internal ECPrivateKeySigner(ECPrivateKeyParameters privateKey)
        {
            signer = new PrivateKeySigner(privateKey, ALGORITHM_NAME);
            curveN = privateKey.Parameters.N;
            halfCurveN = curveN.ShiftRight(1);
        }

        public byte[] Sign(byte[] digest)
        {
            byte[] rawSignature = signer.Sign(digest);
            ECSignature signature = ECSignature.FromBytes(rawSignature);
            signature = PreventMalleability(signature);
            return signature.GetBytes();
        }

        private ECSignature PreventMalleability(ECSignature signature)
        {
            BigInteger s = signature.S.Value;
            if (s.CompareTo(halfCurveN) > 0)
            {
                s = curveN.Subtract(s);
                return new ECSignature(signature.R, new DerInteger(s));
            }
            return signature;
        }
    }
}
