using Gateway.Commands;
using System.Text;

namespace Gateway.Utils;

public static class HyperledgerCommander
{
    private static ILogger _logger = LoggerFactory.Create(builder =>
    {
        builder
            .AddFilter("Default", LogLevel.Debug)
            .AddConsole();
    }).CreateLogger("");

    private static List<HyperledgerConnector> _connectors = new();
    private static int idx = 0;

    static HyperledgerCommander()
    {
        foreach (HyperledgerFabricPeer peer in Configuration.HyperledgerFabric.Peers)
        {
            _connectors.Add(new HyperledgerConnector(Configuration.HyperledgerFabric.ChannelName, Configuration.HyperledgerFabric.ChaincodeName, peer));
        }
    }

    public static async Task Send(string lmid, HyperledgerCommand command, int retry=0)
    {
        try
        {
            idx = (idx + 1) % _connectors.Count;
            HyperledgerConnector connector = _connectors[idx].IsAlive ? _connectors[idx] : _connectors[idx].ReConnect();
            _logger.LogDebug($"{lmid}: Sent to Hyperledger. Peer: {connector.MspId}. Command: {command.Command}. Args: {string.Join("|", command.GetArgs)}");
            byte[] response = await connector.Contract.SubmitTransactionAsync(command.Command, command.GetArgs);
            _logger.LogDebug($"{lmid}: Read from Hyperledger {response.Length} bytes");
        }
        catch (Exception ex)
        {
            if (retry >= _connectors.Count)
            {
                _logger.LogError(lmid, ex);
            }
            else
            {
                await Send(lmid, command, retry + 1);
            }
        }
    }

    public static async Task<string> Receive(string lmid, HyperledgerCommand command, int retry=0)
    {
        try
        {
            idx = (idx + 1) % _connectors.Count;
            HyperledgerConnector connector = _connectors[idx].IsAlive ? _connectors[idx] : _connectors[idx].ReConnect();
            _logger.LogDebug($"{lmid}: Sent to Hyperledger. Peer: {connector.MspId}. Command: {command.Command}. Args: {string.Join("|", command.GetArgs)}");
            byte[] response = await connector.Contract.EvaluateTransactionAsync(command.Command);
            _logger.LogDebug($"{lmid}: Read from Hyperledger {response.Length} bytes");
            return Encoding.UTF8.GetString(response);
        }
        catch (Exception ex)
        {
            if (retry >= _connectors.Count)
            {
                _logger.LogError(lmid, ex);
                return "";
            }
            else
            {
                return await Receive(lmid, command, retry + 1);
            }
        }
    }
}
