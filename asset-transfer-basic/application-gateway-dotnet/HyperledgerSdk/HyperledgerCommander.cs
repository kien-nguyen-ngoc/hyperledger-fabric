using Grpc.Core;
using HyperledgerSdk.Commands;
using Microsoft.Extensions.Logging;
using org.hyperledger.fabric.protos.common;
using org.hyperledger.fabric.protos.peer;
using System.Text;
using System.Text.Json;

namespace HyperledgerSdk;

public static class HyperledgerCommander
{
    private static ILogger _logger;

    private static readonly List<HyperledgerConnector> _connectors = [];
    private static readonly List<HyperledgerConnector> _qsccConnectors = [];
    private static string _channelName;
    private static int _idx = 0;

    public static void Initialize(HyperledgerFabricConfiguration configuration, ILogger logger)
    {
        _logger = logger;
        _channelName = configuration.ChannelName;
        foreach (HyperledgerFabricPeer peer in configuration.Peers)
        {
            _connectors.Add(new HyperledgerConnector(configuration.ChannelName, configuration.ChaincodeName, peer, _logger));
            _qsccConnectors.Add(new HyperledgerConnector(configuration.ChannelName, "qscc", peer, _logger));
        }
    }

    public static async Task<HFTransactionResponse> Submit(string lmid, HyperledgerCommand command, int retry = 0)
    {
        try
        {
            _idx = (_idx + 1) % _connectors.Count;
            HyperledgerConnector connector = _connectors[_idx].IsAlive ? _connectors[_idx] : _connectors[_idx].ReConnect();
            _logger.LogDebug($"{lmid}: Sent to Hyperledger. Peer: {connector.MspId}. Command: {command.Command}. Args: {string.Join("|", command.Args)}");
            byte[] response = await connector.Contract.SubmitTransactionAsync(command.Command, command.Args);
            _logger.LogDebug($"{lmid}: Read from Hyperledger {response.Length} bytes");
            return JsonSerializer.Deserialize<HFTransactionResponse>(Encoding.UTF8.GetString(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(lmid, ex);
            if (ex is org.hyperledger.fabric.client.EndorseException endorseEx && endorseEx.InnerException is RpcException rpcEx && rpcEx.StatusCode is StatusCode.Unavailable or StatusCode.DeadlineExceeded && retry < _connectors.Count)
            {
                return await Submit(lmid, command, retry + 1);
            }
            else
            {
                return new() { TransactionId = null, Error = ex.Message };
            }
        }
    }

    public static async Task<string> Read(string lmid, HyperledgerCommand command, int retry = 0, bool useSystemChainCode = false)
    {
        List<HyperledgerConnector> connectors = useSystemChainCode ? _qsccConnectors : _connectors;
        try
        {
            _idx = (_idx + 1) % connectors.Count;
            HyperledgerConnector connector = connectors[_idx].IsAlive ? connectors[_idx] : connectors[_idx].ReConnect();
            _logger.LogDebug($"{lmid}: Sent to Hyperledger. Peer: {connector.MspId}. Command: {command.Command}. Args: {string.Join("|", command.Args)}");
            byte[] response = await connector.Contract.EvaluateTransactionAsync(command.Command, command.Args);
            _logger.LogDebug($"{lmid}: Read from Hyperledger {response.Length} bytes");
            return Convert.ToHexString(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(lmid, ex);
            if (ex is org.hyperledger.fabric.client.GatewayException gwEx && gwEx.InnerException is RpcException rpcEx && rpcEx.StatusCode is StatusCode.Unavailable or StatusCode.DeadlineExceeded && retry < connectors.Count)
            {
                return await Read(lmid, command, retry + 1);
            }
            else
            {
                if (useSystemChainCode) throw;
                return null;
            }
        }
    }
    public static async Task<HFStatusResponse> Status(string lmid, string trxnId, int retry = 0)
    {
        HFStatusResponse status = new() { TransactionId = trxnId };
        try
        {
            byte[] response = Convert.FromHexString(await Read(lmid, new GetTransactionById() { ChannelName = _channelName, TransactionId = trxnId }, retry, true));
            ProcessedTransaction processedTransaction = ProcessedTransaction.Parser.ParseFrom(response);
            status.ValidationCode = processedTransaction.ValidationCode;
            Payload payload = Payload.Parser.ParseFrom(processedTransaction.TransactionEnvelope.Payload);
            ChannelHeader channelHeader = ChannelHeader.Parser.ParseFrom(payload.Header.ChannelHeader);
            status.SubmitedTimestamp = channelHeader?.Timestamp?.ToDateTime();
            switch ((HeaderType)channelHeader.Type)
            {
                case HeaderType.EndorserTransaction:
                    Transaction transaction = Transaction.Parser.ParseFrom(payload.Data);
                    status.Actions = transaction.Actions.Select(action =>
                    {
                        ChaincodeActionPayload chaincodeActionPayload = ChaincodeActionPayload.Parser.ParseFrom(action.Payload);
                        return new HFAction()
                        {
                            Action = string.Join("|", ChaincodeInvocationSpec.Parser.ParseFrom(ChaincodeProposalPayload.Parser.ParseFrom(chaincodeActionPayload.ChaincodeProposalPayload).Input).ChaincodeSpec.Input.Args.Select(x => x.ToStringUtf8())),
                            Result = ChaincodeAction.Parser.ParseFrom(ProposalResponsePayload.Parser.ParseFrom(chaincodeActionPayload.Action.ProposalResponsePayload).Extension).Response.Payload.ToStringUtf8()
                        };
                    });
                    break;
                default:
                    _logger.LogWarning($"{lmid}: Unhandled transaction type: {channelHeader.Type}");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(lmid, ex);
            status.Error = ex.Message;
        }
        return status;
    }
}
