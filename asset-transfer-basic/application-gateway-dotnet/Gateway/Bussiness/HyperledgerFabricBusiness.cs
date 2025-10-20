using HyperledgerSdk;
using HyperledgerSdk.Commands;
using System.Text;
using System.Text.Json;

namespace Gateway.Bussiness;

public class HyperledgerBusiness
{
    private static ILogger _logger = LoggerFactory.Create(builder =>
    {
        builder
            .AddFilter("Default", LogLevel.Debug)
            .AddConsole();
    }).CreateLogger("");

    static HyperledgerBusiness()
    {
        HyperledgerCommander.Initialize(Configuration.HyperledgerFabric, _logger);
    }

    public static async Task<AccountResponse> GetAccount(string lmid, string accountId)
    {
        HFAccountResponse account = JsonSerializer.Deserialize<HFAccountResponse>(Encoding.UTF8.GetString(Convert.FromHexString(await HyperledgerCommander.Read(lmid, new GetAccountCommand { Id = accountId }))));
        return new AccountResponse { AccountId = account.AccountId, Balance = account.Balance };
    }

    public static async Task<HFStatusResponse> GetTransactionStatus(string lmid, string transactionId)
    {
        return await HyperledgerCommander.Status(lmid, transactionId);
    }

    public static async Task<IEnumerable<AccountResponse>> GetAccounts(string lmid)
    {
        IEnumerable<HFAccountResponse> account = JsonSerializer.Deserialize<IEnumerable<HFAccountResponse>>(Encoding.UTF8.GetString(Convert.FromHexString(await HyperledgerCommander.Read(lmid, new GetAccountsCommand()))));
        return account.Select(x => new AccountResponse { AccountId = x.AccountId, Balance = x.Balance });
    }

    public static async Task<HFTransactionResponse> CreateAccount(string lmid, string accountId)
    {
        HFTransactionResponse response = await HyperledgerCommander.Submit(lmid, new CreateAccountCommand { Id = accountId });
        return response;
    }

    public static async Task<HFTransactionResponse> AddBalance(string lmid, string accountId, decimal amount)
    {
        HFTransactionResponse response = await HyperledgerCommander.Submit(lmid, new AddBalanceCommand { Id = accountId, Amount = amount.ToString() });
        return response;
    }

    public static async Task<HFTransactionResponse> DeductBalance(string lmid, string accountId, decimal amount)
    {
        HFTransactionResponse response = await HyperledgerCommander.Submit(lmid, new DeductBalanceCommand { Id = accountId, Amount = amount.ToString() });
        return response;
    }

    public static async Task<HFTransactionResponse> Transfer(string lmid, string fromAccountId, string toAccountId, decimal amount)
    {
        HFTransactionResponse response = await HyperledgerCommander.Submit(lmid, new TransferCommand { FromId = fromAccountId, ToId = toAccountId, Amount = amount.ToString() });
        return response;
    }
}

public class AccountResponse
{
    public string AccountId { get; set; }
    public decimal Balance { get; set; }
}