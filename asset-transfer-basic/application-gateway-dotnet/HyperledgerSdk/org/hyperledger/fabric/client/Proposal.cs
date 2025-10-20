using Grpc.Core;

namespace org.hyperledger.fabric.client;

public interface IProposal : ISignable
{
    string TransactionId { get; }

    Task<byte[]> EvaluateAsync(CallOptions? options = null);
    Task<ITransaction> EndorseAsync(CallOptions? options = null);

    public interface IBuilder : IBuilder<IProposal>
    {
        IBuilder AddArguments(params byte[][] args);
        IBuilder AddArguments(params string[] args);
        IBuilder PutAllTransient(IDictionary<string, byte[]> transientData);
        IBuilder PutTransient(string key, byte[] value);
        IBuilder PutTransient(string key, string value);
        IBuilder SetEndorsingOrganizations(params string[] mspids);
    }
}
