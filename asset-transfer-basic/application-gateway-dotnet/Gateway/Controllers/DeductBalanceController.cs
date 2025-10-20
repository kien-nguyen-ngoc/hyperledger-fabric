using Gateway.Bussiness;
using HyperledgerSdk;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeductBalanceController : ControllerBase
    {
        private readonly ILogger<AddBalanceController> _logger;

        public DeductBalanceController(ILogger<AddBalanceController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<HFTransactionResponse> Post(string id, decimal amount)
        {
            return await HyperledgerBusiness.DeductBalance("", id, amount);
        }
    }
}
