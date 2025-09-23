/*
 * Copyright 2022 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */


/*
 * Copyright 2022 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using org.hyperledger.fabric.protos.orderer;

namespace org.hyperledger.fabric.client;

public class StartPositionBuilder : IBuilder<SeekPosition>
{
    private readonly SeekPosition builder = new() { NextCommit = new SeekNextCommit() };

    public StartPositionBuilder StartBlock(long blockNumber)
    {
        var specified = new SeekSpecified { Number = (ulong)blockNumber };
        builder.Specified = specified;
        return this;
    }

    public SeekPosition Build()
    {
        return builder;
    }
}
