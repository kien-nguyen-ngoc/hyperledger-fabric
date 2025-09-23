namespace Gateway;

public class Configuration
{
    private static readonly IConfiguration _appSettings = new ConfigurationBuilder()
        .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "baseconfiguration.json"), true)
        .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "baseconfiguration.local.json"), true)
        .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "logging.json"), true)
        .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "logging.local.json"), true)
        .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), true)
        .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.local.json"), true)
        .AddEnvironmentVariables()
        .Build();

    public static readonly HyperledgerFabricConfiguration HyperledgerFabric = _appSettings.GetSection("HyperledgerFabric").Get<HyperledgerFabricConfiguration>();
}

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