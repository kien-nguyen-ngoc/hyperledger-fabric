using Google.Protobuf;
using Grpc.Core;
using org.hyperledger.fabric.protos.common;
using org.hyperledger.fabric.protos.peer;

namespace org.hyperledger.fabric.client;

public class Network : INetwork
{
    private readonly GatewayClient client;
    private readonly SigningIdentity signingIdentity;
    private readonly ByteString tlsCertificateHash;

    internal Network(
        GatewayClient client,
        SigningIdentity signingIdentity,
        string channelName,
        ByteString tlsCertificateHash)
    {
        ArgumentNullException.ThrowIfNull(channelName, "network name");

        this.client = client;
        this.signingIdentity = signingIdentity;
        Name = channelName;
        this.tlsCertificateHash = tlsCertificateHash;
    }

    public IContract GetContract(string chaincodeName, string contractName)
    {
        return new ContractImpl(client, signingIdentity, Name, chaincodeName, contractName);
    }

    public IContract GetContract(string chaincodeName)
    {
        return new ContractImpl(client, signingIdentity, Name, chaincodeName);
    }

    public string Name { get; }

    public IAsyncEnumerable<IChaincodeEvent> GetChaincodeEvents(string chaincodeName, CallOptions? options = null)
    {
        return NewChaincodeEventsRequest(chaincodeName).Build().GetEvents(options);
    }

    public IChaincodeEventsRequest.IBuilder NewChaincodeEventsRequest(string chaincodeName)
    {
        return new ChaincodeEventsBuilder(client, signingIdentity, Name, chaincodeName);
    }

    public IAsyncEnumerable<Block> GetBlockEvents(CallOptions? options = null)
    {
        return NewBlockEventsRequest().Build().GetEvents(options);
    }

    public IBlockEventsRequest.IBuilder NewBlockEventsRequest()
    {
        return new BlockEventsBuilder(client, signingIdentity, Name, tlsCertificateHash);
    }

    public IAsyncEnumerable<FilteredBlock> GetFilteredBlockEvents(CallOptions? options = null)
    {
        return NewFilteredBlockEventsRequest().Build().GetEvents(options);
    }

    public IFilteredBlockEventsRequest.IBuilder NewFilteredBlockEventsRequest()
    {
        return new FilteredBlockEventsBuilder(client, signingIdentity, Name, tlsCertificateHash);
    }

    public IAsyncEnumerable<BlockAndPrivateData> GetBlockAndPrivateDataEvents(CallOptions? options = null)
    {
        return NewBlockAndPrivateDataEventsRequest().Build().GetEvents(options);
    }

    public IBlockAndPrivateDataEventsRequest.IBuilder NewBlockAndPrivateDataEventsRequest()
    {
        return new BlockAndPrivateDataEventsBuilder(client, signingIdentity, Name, tlsCertificateHash);
    }
}
