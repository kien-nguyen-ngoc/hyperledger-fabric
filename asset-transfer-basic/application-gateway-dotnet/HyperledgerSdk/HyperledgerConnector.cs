using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.Logging;
using org.hyperledger.fabric.client;
using org.hyperledger.fabric.client.identity;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace HyperledgerSdk;

public class HyperledgerConnector
{
    private readonly ILogger _logger;
    private readonly string _mspId;
    private readonly string _channelName;
    private readonly string _chaincodeName;
    private readonly string _certPath;
    private readonly string _keyPath;
    private readonly string _tlsCertPath;
    private readonly string _endpoint;
    private readonly string _hostOverride;
    private IContract _contract;
    private GrpcChannel _channel;

    public HyperledgerConnector(string channelName, string chaincodeName, HyperledgerFabricPeer peer, ILogger logger)
    {
        _logger = logger;
        _channelName = channelName;
        _chaincodeName = chaincodeName;
        _mspId = peer.MspId;
        _certPath = peer.CertPath;
        _keyPath = peer.KeyPath;
        _tlsCertPath = peer.TlsCertPath;
        _endpoint = peer.Endpoint;
        _hostOverride = peer.HostOverride;
        _channel = CreateGrpcConnection();
        _contract = Connect()?.GetNetwork(_channelName).GetContract(_chaincodeName);
    }

    public IContract Contract => _contract;
    public GrpcChannel Channel => _channel;
    public string MspId => _mspId;

    public bool IsAlive => _contract != null && _channel?.State is Grpc.Core.ConnectivityState.Idle or Grpc.Core.ConnectivityState.Connecting or Grpc.Core.ConnectivityState.Ready;

    public HyperledgerConnector ReConnect()
    {
        _channel.Dispose();
        _channel = CreateGrpcConnection();
        _contract = Connect()?.GetNetwork(_channelName).GetContract(_chaincodeName);
        return this;
    }

    public IGateway Connect()
    {
        try
        {
            return _channel?.State is Grpc.Core.ConnectivityState.Idle or Grpc.Core.ConnectivityState.Connecting or Grpc.Core.ConnectivityState.Ready ? IGateway.NewInstance()
                    .Identity(CreateIdentity())
                    .Signer(CreateSigner())
                    .Hash(Hash.Sha256)
                    .Connection(_channel)
                    .EvaluateOptions(options => options.WithDeadline(DateTime.UtcNow.Add(TimeSpan.FromSeconds(5))))
                    .EndorseOptions(options => options.WithDeadline(DateTime.UtcNow.Add(TimeSpan.FromSeconds(15))))
                    .SubmitOptions(options => options.WithDeadline(DateTime.UtcNow.Add(TimeSpan.FromSeconds(5))))
                    .CommitStatusOptions(options => options.WithDeadline(DateTime.UtcNow.Add(TimeSpan.FromSeconds(60))))
                    .Connect() : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return null;
        }
    }

    private GrpcChannel CreateGrpcConnection()
    {
        var handler = new SocketsHttpHandler
        {
            SslOptions = new SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (httpRequestMessage, cert, chain, errors) =>
                {
                    if (errors == SslPolicyErrors.None)
                        return true;

                    if (chain != null && cert != null)
                    {
                        var caCert = X509CertificateLoader.LoadCertificateFromFile(_tlsCertPath);
                        chain.ChainPolicy.ExtraStore.Add(caCert);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                        return chain.Build(new X509Certificate2(cert)) && chain.ChainElements[^1].Certificate.Thumbprint == caCert.Thumbprint;
                    }

                    return false;
                },
                TargetHost = _hostOverride,
            }
        };

        try
        {
            return GrpcChannel.ForAddress(_endpoint, new GrpcChannelOptions
            {
                HttpHandler = handler,
                ServiceConfig = new ServiceConfig
                {
                    MethodConfigs = { new MethodConfig { Names = { MethodName.Default }, RetryPolicy = null } }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return null;
        }
    }

    private X509Identity CreateIdentity()
    {
        var certificate = X509CertificateLoader.LoadCertificateFromFile(_certPath);
        return new X509Identity(_mspId, certificate);
    }

    private ISigner CreateSigner()
    {
        string keyFile = Directory.GetFiles(_keyPath).First();
        return Signers.NewPrivateKeySigner(Identities.ReadPrivateKey(File.ReadAllText(keyFile)));
    }
}
