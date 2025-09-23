/*
 * Copyright 2020 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace org.hyperledger.fabric.client.identity
{
    /// <summary>
    /// A client identity described by an X.509 certificate. The <see cref="Identities"/> class provides static methods to create
    /// an <c>X509Certificate2</c> object from PEM-format data.
    /// </summary>
    public sealed class X509Identity : IIdentity, IEquatable<X509Identity>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mspId">A membership service provider identifier.</param>
        /// <param name="certificate">An X.509 certificate.</param>
        public X509Identity(string mspId, X509Certificate2 certificate)
        {
            MspId = mspId;
            Certificate = certificate;
            Credentials = Encoding.UTF8.GetBytes(Identities.ToPemString(certificate));
        }

        /// <summary>
        /// Get the certificate for this identity.
        /// </summary>
        public X509Certificate2 Certificate { get; }

        public string MspId { get; }

        public byte[] Credentials { get; }

        public bool Equals(X509Identity? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return MspId == other.MspId && Credentials.SequenceEqual(other.Credentials);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is X509Identity other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (MspId?.GetHashCode() ?? 0);
                hash = hash * 23 + (Credentials != null ? Credentials.Aggregate(17, (current, b) => current * 23 + b) : 0);
                return hash;
            }
        }
    }
}
