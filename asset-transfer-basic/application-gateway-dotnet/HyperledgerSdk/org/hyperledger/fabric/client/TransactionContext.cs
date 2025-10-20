using Google.Protobuf;
using org.hyperledger.fabric.protos.common;
using System.Security.Cryptography;

namespace org.hyperledger.fabric.client;

public class TransactionContext
{
    private const int NonceLength = 24;

    private readonly SigningIdentity signingIdentity;
    private readonly byte[] nonce;

    internal TransactionContext(SigningIdentity signingIdentity)
    {
        this.signingIdentity = signingIdentity;
        nonce = NewNonce();
        TransactionId = NewTransactionId();
        SignatureHeader = NewSignatureHeader();
    }

    private static byte[] NewNonce()
    {
        return RandomNumberGenerator.GetBytes(NonceLength);
    }

    private string NewTransactionId()
    {
        byte[] saltedCreator = GatewayUtils.Concat(nonce, signingIdentity.Creator);
        byte[] rawTransactionId = SHA256.HashData(saltedCreator);
        return Convert.ToHexString(rawTransactionId).ToLowerInvariant();
    }

    private SignatureHeader NewSignatureHeader()
    {
        return new SignatureHeader
        {
            Creator = ByteString.CopyFrom(signingIdentity.Creator),
            Nonce = ByteString.CopyFrom(nonce)
        };
    }

    public string TransactionId { get; }
    public SignatureHeader SignatureHeader { get; }
}
