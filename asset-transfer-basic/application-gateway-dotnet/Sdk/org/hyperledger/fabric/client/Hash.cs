/*
 * Copyright 2020 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.Security.Cryptography;

namespace org.hyperledger.fabric.client;

/// <summary>
/// Hash function implementations used to generate a digest of a supplied message.
/// </summary>
public static class Hash
{
    /// <summary>
    /// Returns the input message unchanged. This can be used if the signing implementation requires the full message
    /// bytes, not just a pre-generated digest, such as Ed25519.
    /// </summary>
    public static readonly Func<byte[], byte[]> None = message => message;

    /// <summary>
    /// SHA-256 hash.
    /// </summary>
    public static readonly Func<byte[], byte[]> Sha256 = message => Digest(HashAlgorithmName.SHA256, message);

    /// <summary>
    /// SHA-384 hash.
    /// </summary>
    public static readonly Func<byte[], byte[]> Sha384 = message => Digest(HashAlgorithmName.SHA384, message);

    private static byte[] Digest(HashAlgorithmName algorithm, byte[] message)
    {
        using var hashAlgorithm = IncrementalHash.CreateHash(algorithm);
        hashAlgorithm.AppendData(message);
        return hashAlgorithm.GetHashAndReset();
    }
}
