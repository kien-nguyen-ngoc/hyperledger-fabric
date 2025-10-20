using Grpc.Core;

namespace org.hyperledger.fabric.client;

public class DefaultCallOptions
{
    private readonly Func<CallOptions, CallOptions> evaluate;
    private readonly Func<CallOptions, CallOptions> endorse;
    private readonly Func<CallOptions, CallOptions> submit;
    private readonly Func<CallOptions, CallOptions> commitStatus;
    private readonly Func<CallOptions, CallOptions> chaincodeEvents;
    private readonly Func<CallOptions, CallOptions> blockEvents;
    private readonly Func<CallOptions, CallOptions> filteredBlockEvents;
    private readonly Func<CallOptions, CallOptions> blockAndPrivateDataEvents;

    private DefaultCallOptions(Builder builder)
    {
        evaluate = builder.Evaluate;
        endorse = builder.Endorse;
        submit = builder.Submit;
        commitStatus = builder.CommitStatus;
        chaincodeEvents = builder.ChaincodeEvents;
        blockEvents = builder.BlockEvents;
        filteredBlockEvents = builder.FilteredBlockEvents;
        blockAndPrivateDataEvents = builder.BlockAndPrivateDataEvents;
    }

    public static Builder NewBuilder()
    {
        return new Builder();
    }

    public CallOptions GetEvaluate(CallOptions? additional) => ApplyOptions(additional, evaluate);
    public CallOptions GetEndorse(CallOptions? additional) => ApplyOptions(additional, endorse);
    public CallOptions GetSubmit(CallOptions? additional) => ApplyOptions(additional, submit);
    public CallOptions GetCommitStatus(CallOptions? additional) => ApplyOptions(additional, commitStatus);
    public CallOptions GetChaincodeEvents(CallOptions? additional) => ApplyOptions(additional, chaincodeEvents);
    public CallOptions GetBlockEvents(CallOptions? additional) => ApplyOptions(additional, blockEvents);
    public CallOptions GetFilteredBlockEvents(CallOptions? additional) => ApplyOptions(additional, filteredBlockEvents);
    public CallOptions GetBlockAndPrivateDataEvents(CallOptions? additional) => ApplyOptions(additional, blockAndPrivateDataEvents);

    private static CallOptions ApplyOptions(CallOptions? callOptions, Func<CallOptions, CallOptions> func)
    {
        var result = callOptions ?? new CallOptions();
        if (func != null)
            result = func(result);
        return result;
    }

    public sealed class Builder
    {
        internal Func<CallOptions, CallOptions> Evaluate { get; private set; }
        internal Func<CallOptions, CallOptions> Endorse { get; private set; }
        internal Func<CallOptions, CallOptions> Submit { get; private set; }
        internal Func<CallOptions, CallOptions> CommitStatus { get; private set; }
        internal Func<CallOptions, CallOptions> ChaincodeEvents { get; private set; }
        internal Func<CallOptions, CallOptions> BlockEvents { get; private set; }
        internal Func<CallOptions, CallOptions> FilteredBlockEvents { get; private set; }
        internal Func<CallOptions, CallOptions> BlockAndPrivateDataEvents { get; private set; }

        internal Builder() { }

        public Builder WithEvaluate(Func<CallOptions, CallOptions> options)
        {
            Evaluate = options;
            return this;
        }

        public Builder WithEndorse(Func<CallOptions, CallOptions> options)
        {
            Endorse = options;
            return this;
        }

        public Builder WithSubmit(Func<CallOptions, CallOptions> options)
        {
            Submit = options;
            return this;
        }

        public Builder WithCommitStatus(Func<CallOptions, CallOptions> options)
        {
            CommitStatus = options;
            return this;
        }

        public Builder WithChaincodeEvents(Func<CallOptions, CallOptions> options)
        {
            ChaincodeEvents = options;
            return this;
        }

        public Builder WithBlockEvents(Func<CallOptions, CallOptions> options)
        {
            BlockEvents = options;
            return this;
        }

        public Builder WithFilteredBlockEvents(Func<CallOptions, CallOptions> options)
        {
            FilteredBlockEvents = options;
            return this;
        }

        public Builder WithBlockAndPrivateDataEvents(Func<CallOptions, CallOptions> options)
        {
            BlockAndPrivateDataEvents = options;
            return this;
        }

        public DefaultCallOptions Build()
        {
            return new DefaultCallOptions(this);
        }
    }
}
