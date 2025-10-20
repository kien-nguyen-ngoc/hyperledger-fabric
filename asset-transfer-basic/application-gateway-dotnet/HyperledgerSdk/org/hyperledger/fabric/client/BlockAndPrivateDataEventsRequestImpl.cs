using Grpc.Core;
using org.hyperledger.fabric.protos.common;
using org.hyperledger.fabric.protos.peer;

namespace org.hyperledger.fabric.client;

public class BlockAndPrivateDataEventsRequestImpl : SignableBlockEventsRequest, IBlockAndPrivateDataEventsRequest
{
    private readonly GatewayClient client;

    internal BlockAndPrivateDataEventsRequestImpl(
        GatewayClient client, SigningIdentity signingIdentity, Envelope request) : base(signingIdentity, request)
    {
        this.client = client;
    }

    public IAsyncEnumerable<BlockAndPrivateData> GetEvents(CallOptions? options = null)
    {
        Envelope signedRequest = GetSignedRequest();
        IAsyncEnumerable<DeliverResponse> responseIter = client.BlockAndPrivateDataEvents(signedRequest, options);

        return responseIter.Select(response =>
        {
            if (response.TypeCase == DeliverResponse.TypeOneofCase.Status)
            {
                throw new Exception($"Unexpected status response: {response.Status}");
            }

            return response.BlockAndPrivateData;
        });
    }
}
