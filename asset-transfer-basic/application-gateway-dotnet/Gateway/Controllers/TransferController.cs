using Gateway.Bussiness;
using HyperledgerSdk;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransferController : ControllerBase
    {
        private readonly ILogger<AddBalanceController> _logger;

        public TransferController(ILogger<AddBalanceController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<HFTransactionResponse> Post(string fromId, string toId, decimal amount)
        {
            return await HyperledgerBusiness.Transfer("", fromId, toId, amount);
        }
    }
}
