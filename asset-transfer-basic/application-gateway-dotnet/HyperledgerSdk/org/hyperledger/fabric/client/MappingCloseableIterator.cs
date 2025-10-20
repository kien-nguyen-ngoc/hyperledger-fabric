namespace org.hyperledger.fabric.client;

public class MappingCloseableAsyncEnumerator<T, R> : ICloseableAsyncEnumerator<R>
{
    private readonly ICloseableAsyncEnumerator<T> enumerator;
    private readonly Func<T, R> mapper;

    public MappingCloseableAsyncEnumerator(ICloseableAsyncEnumerator<T> enumerator, Func<T, R> mapper)
    {
        this.enumerator = enumerator;
        this.mapper = mapper;
    }

    public R Current => mapper(enumerator.Current);

    public ValueTask DisposeAsync()
    {
        return enumerator.DisposeAsync();
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return enumerator.MoveNextAsync();
    }
}
