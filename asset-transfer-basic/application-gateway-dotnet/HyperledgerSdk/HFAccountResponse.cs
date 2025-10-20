using System.Text.Json.Serialization;

namespace HyperledgerSdk;

public class HFAccountResponse
{
    public string AccountId { get; set; }
    public decimal Balance { get; set; }
}