using Google.Protobuf;
using org.hyperledger.fabric.client.identity;
using org.hyperledger.fabric.protos.msp;

namespace org.hyperledger.fabric.client;

public class SigningIdentity
{
    private readonly ISigner signer;
    private readonly SerializedIdentity creator;

    internal SigningIdentity(IIdentity identity, Func<byte[], byte[]> hash, ISigner signer)
    {
        ArgumentNullException.ThrowIfNull(identity, "No identity supplied");
        ArgumentNullException.ThrowIfNull(hash, "No hash implementation supplied");
        ArgumentNullException.ThrowIfNull(signer, "No signing implementation supplied");

        Identity = identity;
        Hash = hash;
        this.signer = signer;

        creator = GatewayUtils.NewSerializedIdentity(identity);
    }

    public IIdentity Identity { get; }
    public Func<byte[], byte[]> Hash { get; }

    public byte[] Sign(byte[] digest)
    {
        return signer.Sign(digest);
    }

    public byte[] Creator => creator.ToByteArray();
}
