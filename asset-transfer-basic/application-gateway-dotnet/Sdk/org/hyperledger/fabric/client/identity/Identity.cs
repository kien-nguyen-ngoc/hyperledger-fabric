/*
 * Copyright 2020 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

namespace org.hyperledger.fabric.client.identity
{
    /// <summary>
    /// Represents a client identity used to interact with a Fabric network. The identity consists of an identifier for the
    /// organization to which the identity belongs, and implementation-specific credentials describing the identity.
    /// </summary>
    public interface IIdentity
    {
        /// <summary>
        /// Member services provider to which this identity is associated.
        /// </summary>
        string MspId { get; }

        /// <summary>
        /// Implementation-specific credentials.
        /// </summary>
        byte[] Credentials { get; }
    }
}
