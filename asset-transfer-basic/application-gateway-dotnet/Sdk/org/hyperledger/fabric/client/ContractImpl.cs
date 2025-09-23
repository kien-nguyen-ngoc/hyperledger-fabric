/*
 * Copyright 2019 IBM All Rights Reserved.
 *
 * SPDX-License-Identifier: Apache-2.0
 */

namespace org.hyperledger.fabric.client;

public class ContractImpl : IContract
{
    private readonly GatewayClient client;
    private readonly SigningIdentity signingIdentity;
    private readonly string channelName;

    internal ContractImpl(
        GatewayClient client,
        SigningIdentity signingIdentity,
        string channelName,
        string chaincodeName,
        string? contractName = null)
    {
        ArgumentNullException.ThrowIfNull(chaincodeName, nameof(chaincodeName));

        this.client = client;
        this.signingIdentity = signingIdentity;
        this.channelName = channelName;
        ChaincodeName = chaincodeName;
        ContractName = contractName;
    }

    public string ChaincodeName { get; }
    public string? ContractName { get; }

    public async Task<byte[]> SubmitTransactionAsync(string name)
    {
        var transaction = await NewProposal(name).Build().EndorseAsync();
        await transaction.SubmitAsync();
        return transaction.Result;
    }

    public async Task<byte[]> SubmitTransactionAsync(string name, params string[] args)
    {
        var transaction = await NewProposal(name).AddArguments(args).Build().EndorseAsync();
        await transaction.SubmitAsync();
        return transaction.Result;
    }

    public async Task<byte[]> SubmitTransactionAsync(string name, params byte[][] args)
    {
        var transaction = await NewProposal(name).AddArguments(args).Build().EndorseAsync();
        await transaction.SubmitAsync();
        return transaction.Result;
    }

    public Task<byte[]> EvaluateTransactionAsync(string name)
    {
        return NewProposal(name).Build().EvaluateAsync();
    }

    public Task<byte[]> EvaluateTransactionAsync(string name, params string[] args)
    {
        return NewProposal(name).AddArguments(args).Build().EvaluateAsync();
    }

    public Task<byte[]> EvaluateTransactionAsync(string name, params byte[][] args)
    {
        return NewProposal(name).AddArguments(args).Build().EvaluateAsync();
    }

    public IProposal.IBuilder NewProposal(string transactionName)
    {
        string qualifiedTxName = QualifiedTransactionName(transactionName);
        return new ProposalBuilder(client, signingIdentity, channelName, ChaincodeName, qualifiedTxName);
    }

    private string QualifiedTransactionName(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        return !string.IsNullOrEmpty(ContractName) ? $"{ContractName}:{name}" : name;
    }
}
