namespace org.hyperledger.fabric.client;

public interface IContract
{
    string ChaincodeName { get; }
    string ContractName { get; }

    Task<byte[]> SubmitTransactionAsync(string name);
    Task<byte[]> SubmitTransactionAsync(string name, params string[] args);
    Task<byte[]> SubmitTransactionAsync(string name, params byte[][] args);

    Task<byte[]> EvaluateTransactionAsync(string name);
    Task<byte[]> EvaluateTransactionAsync(string name, params string[] args);
    Task<byte[]> EvaluateTransactionAsync(string name, params byte[][] args);

    IProposal.IBuilder NewProposal(string transactionName);
}
