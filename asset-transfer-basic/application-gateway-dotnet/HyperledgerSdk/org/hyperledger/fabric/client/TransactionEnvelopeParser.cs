using Google.Protobuf;
using org.hyperledger.fabric.protos.common;
using org.hyperledger.fabric.protos.peer;

namespace org.hyperledger.fabric.client;

public class TransactionEnvelopeParser
{
    public TransactionEnvelopeParser(Envelope envelope)
    {
        try
        {
            var payload = Payload.Parser.ParseFrom(envelope.Payload);
            ChannelName = ParseChannelName(payload.Header);
            Result = ParseResult(payload);
        }
        catch (InvalidProtocolBufferException e)
        {
            throw new ArgumentException("Invalid transaction payload", e);
        }
    }

    public string ChannelName { get; }
    public ByteString Result { get; }

    private string ParseChannelName(Header header)
    {
        var channelHeader = ChannelHeader.Parser.ParseFrom(header.ChannelHeader);
        return channelHeader.ChannelId;
    }

    private ByteString ParseResult(Payload payload)
    {
        var transaction = protos.peer.Transaction.Parser.ParseFrom(payload.Data);
        var parseExceptions = new List<Exception>();

        foreach (var transactionAction in transaction.Actions)
        {
            try
            {
                return ParseResult(transactionAction);
            }
            catch (InvalidProtocolBufferException ex)
            {
                parseExceptions.Add(ex);
            }
        }

        var e = new ArgumentException("No proposal response found");
        foreach (var suppressed in parseExceptions)
        {
            e.Data.Add(suppressed, null);
        }
        throw e;
    }

    private ByteString ParseResult(TransactionAction transactionAction)
    {
        var actionPayload = ChaincodeActionPayload.Parser.ParseFrom(transactionAction.Payload);
        var responsePayload = ProposalResponsePayload.Parser.ParseFrom(actionPayload.Action.ProposalResponsePayload);
        var chaincodeAction = ChaincodeAction.Parser.ParseFrom(responsePayload.Extension);
        return chaincodeAction.Response.Payload;
    }
}
