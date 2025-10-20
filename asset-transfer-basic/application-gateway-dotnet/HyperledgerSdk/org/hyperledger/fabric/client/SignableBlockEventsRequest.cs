using Google.Protobuf;
using org.hyperledger.fabric.protos.common;

namespace org.hyperledger.fabric.client;

public abstract class SignableBlockEventsRequest : ISignable
{
    private readonly SigningIdentity signingIdentity;
    private Envelope request;

    protected SignableBlockEventsRequest(SigningIdentity signingIdentity, Envelope request)
    {
        this.signingIdentity = signingIdentity;
        this.request = request;
    }

    protected Envelope GetSignedRequest()
    {
        if (!IsSigned())
        {
            byte[] digest = GetDigest();
            byte[] signature = signingIdentity.Sign(digest);
            SetSignature(signature);
        }

        return request;
    }

    public byte[] GetBytes()
    {
        return request.ToByteArray();
    }

    public byte[] GetDigest()
    {
        byte[] message = request.Payload.ToByteArray();
        return signingIdentity.Hash(message);
    }

    public void SetSignature(byte[] signature)
    {
        request.Signature = ByteString.CopyFrom(signature);
    }

    private bool IsSigned()
    {
        return !request.Signature.IsEmpty;
    }
}
