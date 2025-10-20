using Grpc.Core;
using org.hyperledger.fabric.protos.gateway;
using System.Text;

namespace org.hyperledger.fabric.client;

public class GrpcStatus(Grpc.Core.Status status, Metadata trailers)
{
    private readonly Grpc.Core.Status status = status;
    private readonly Metadata trailers = trailers ?? [];

    public Grpc.Core.Status Status => status;

    public IEnumerable<ErrorDetail> Details => trailers.Select(trailer => new ErrorDetail() { Message = trailer.IsBinary ? Encoding.ASCII.GetString(trailer.ValueBytes) : trailer.Value });

    public override string ToString()
    {
        return $"[{status.StatusCode}] {status.Detail}. ({string.Join("; ", Details.Select(m => m.Message))})";
    }
}
