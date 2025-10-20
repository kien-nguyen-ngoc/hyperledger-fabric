namespace HyperledgerSdk;

public class HyperledgerFabricConfiguration
{
    public string ChannelName { get; set; }
    public string ChaincodeName { get; set; }
    public IEnumerable<HyperledgerFabricPeer> Peers { get; set; }
}

public class HyperledgerFabricPeer
{
    public string MspId { get; set; }
    public string CertPath { get; set; }
    public string KeyPath { get; set; }
    public string TlsCertPath { get; set; }
    public string Endpoint { get; set; }
    public string HostOverride { get; set; }
}