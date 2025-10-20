using System.Text.Json;
using System.Text.Json.Nodes;

namespace org.hyperledger.fabric.client;

/// <summary>
/// Checkpointer implementation backed by persistent file storage.
/// It can be used to checkpoint progress after successfully processing events, allowing eventing to be resumed from this point.
/// </summary>
public sealed class FileCheckpointer : ICheckpointer, IAsyncDisposable
{
    private const string ConfigKeyBlock = "blockNumber";
    private const string ConfigKeyTransactionId = "transactionId";

    private readonly string path;
    private readonly FileStream fileStream;

    /// <summary>
    /// To create a checkpointer instance backed by persistent file storage.
    /// </summary>
    /// <param name="path">Path of the file which has to store the checkpointer state.</param>
    /// <exception cref="IOException">if the file cannot be opened, is unwritable, or contains invalid checkpointer state data.</exception>
    public FileCheckpointer(string path)
    {
        this.path = path;
        fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        if (fileStream.Length > 0)
        {
            Load();
        }
        else
        {
            // If the file is new, write initial empty state
            SaveAsync().AsTask().GetAwaiter().GetResult();
        }
    }

    public long? BlockNumber { get; private set; }
    public string TransactionId { get; private set; }

    public async ValueTask CheckpointBlockAsync(long blockNumber)
    {
        BlockNumber = blockNumber + 1;
        TransactionId = null;
        await SaveAsync();
    }

    public async ValueTask CheckpointTransactionAsync(long blockNumber, string transactionId)
    {
        BlockNumber = blockNumber;
        TransactionId = transactionId;
        await SaveAsync();
    }

    public ValueTask CheckpointChaincodeEventAsync(IChaincodeEvent @event)
    {
        return CheckpointTransactionAsync(@event.BlockNumber, @event.TransactionId);
    }

    private void Load()
    {
        fileStream.Position = 0;
        try
        {
            var data = JsonNode.Parse(fileStream);
            if (data == null)
                return;

            if (data[ConfigKeyBlock] != null)
                BlockNumber = data[ConfigKeyBlock]!.GetValue<long>();
            else
                BlockNumber = null;

            if (data[ConfigKeyTransactionId] != null)
                TransactionId = data[ConfigKeyTransactionId]!.GetValue<string>();
            else
                TransactionId = null;
        }
        catch (JsonException e)
        {
            throw new IOException($"Failed to parse checkpoint data from file: {path}", e);
        }
    }

    private async ValueTask SaveAsync()
    {
        var jsonObject = new JsonObject();
        if (BlockNumber.HasValue)
            jsonObject[ConfigKeyBlock] = BlockNumber.Value;
        if (TransactionId != null)
            jsonObject[ConfigKeyTransactionId] = TransactionId;

        fileStream.Position = 0;
        fileStream.SetLength(0); // Truncate file
        await JsonSerializer.SerializeAsync(fileStream, jsonObject);
        await fileStream.FlushAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await fileStream.DisposeAsync();
    }

    /// <summary>
    /// Commits file changes to the storage device.
    /// </summary>
    /// <exception cref="IOException">if an I/O error occurs.</exception>
    public void Sync()
    {
        fileStream.Flush(true);
    }
}
