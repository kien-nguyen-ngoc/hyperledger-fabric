using Gateway.Bussiness;
using HyperledgerSdk;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetTransactionController : ControllerBase
    {
        private readonly ILogger<AddBalanceController> _logger;

        public GetTransactionController(ILogger<AddBalanceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<HFStatusResponse> Post(string id)
        {
            return await HyperledgerBusiness.GetTransactionStatus("", id);
        }
    }
}
