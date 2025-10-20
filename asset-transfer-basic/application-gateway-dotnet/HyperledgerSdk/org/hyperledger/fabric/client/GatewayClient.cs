using Grpc.Core;
using org.hyperledger.fabric.protos.common;
using org.hyperledger.fabric.protos.gateway;
using org.hyperledger.fabric.protos.peer;
using static org.hyperledger.fabric.protos.peer.Deliver;

namespace org.hyperledger.fabric.client;

public class GatewayClient
{
    private readonly protos.gateway.Gateway.GatewayClient gatewayStub;
    private readonly DeliverClient deliverStub;
    private readonly DefaultCallOptions defaultOptions;

    internal GatewayClient(ChannelBase channel, DefaultCallOptions defaultOptions)
    {
        ArgumentNullException.ThrowIfNull(channel, "No connection details supplied");
        ArgumentNullException.ThrowIfNull(defaultOptions, nameof(defaultOptions));

        gatewayStub = new protos.gateway.Gateway.GatewayClient(channel);
        deliverStub = new DeliverClient(channel);
        this.defaultOptions = defaultOptions;
    }

    public async Task<EvaluateResponse> EvaluateAsync(EvaluateRequest request, CallOptions? options)
    {
        try
        {
            return await gatewayStub.EvaluateAsync(request, defaultOptions.GetEvaluate(options));
        }
        catch (RpcException e)
        {
            throw new GatewayException(e);
        }
    }

    public async Task<EndorseResponse> EndorseAsync(EndorseRequest request, CallOptions? options)
    {
        try
        {
            return await gatewayStub.EndorseAsync(request, defaultOptions.GetEndorse(options));
        }
        catch (RpcException e)
        {
            throw new EndorseException(request.TransactionId, e);
        }
    }

    public async Task<protos.gateway.SubmitResponse> SubmitAsync(protos.gateway.SubmitRequest request, CallOptions? options)
    {
        try
        {
            return await gatewayStub.SubmitAsync(request, defaultOptions.GetSubmit(options));
        }
        catch (RpcException e)
        {
            throw new SubmitException(request.TransactionId, e);
        }
    }

    public async Task<CommitStatusResponse> CommitStatusAsync(SignedCommitStatusRequest request, CallOptions? options)
    {
        try
        {
            return await gatewayStub.CommitStatusAsync(request, defaultOptions.GetCommitStatus(options));
        }
        catch (RpcException e)
        {
            try
            {
                var req = CommitStatusRequest.Parser.ParseFrom(request.Request);
                throw new CommitStatusException(req.TransactionId, e);
            }
            catch (Exception protoErr)
            {
                var commitErr = new CommitStatusException("", e);
                throw new AggregateException(commitErr, protoErr);
            }
        }
    }

    public IAsyncEnumerable<ChaincodeEventsResponse> ChaincodeEvents(SignedChaincodeEventsRequest request, CallOptions? options)
    {
        var call = gatewayStub.ChaincodeEvents(request, defaultOptions.GetChaincodeEvents(options));
        return call.ResponseStream.ReadAllAsync();
    }

    public IAsyncEnumerable<DeliverResponse> BlockEvents(Envelope request, CallOptions? options)
    {
        var call = deliverStub.Deliver(defaultOptions.GetBlockEvents(options));
        return DuplexStreamingCall(call, request);
    }

    public IAsyncEnumerable<DeliverResponse> FilteredBlockEvents(Envelope request, CallOptions? options)
    {
        var call = deliverStub.DeliverFiltered(defaultOptions.GetFilteredBlockEvents(options));
        return DuplexStreamingCall(call, request);
    }

    public IAsyncEnumerable<DeliverResponse> BlockAndPrivateDataEvents(Envelope request, CallOptions? options)
    {
        var call = deliverStub.DeliverWithPrivateData(defaultOptions.GetBlockAndPrivateDataEvents(options));
        return DuplexStreamingCall(call, request);
    }

    private async IAsyncEnumerable<TResp> DuplexStreamingCall<TReq, TResp>(AsyncDuplexStreamingCall<TReq, TResp> call, TReq request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken token = default)
    {
        await call.RequestStream.WriteAsync(request);
        await foreach (var resp in call.ResponseStream.ReadAllAsync(token))
        {
            yield return resp;
        }
    }
}
