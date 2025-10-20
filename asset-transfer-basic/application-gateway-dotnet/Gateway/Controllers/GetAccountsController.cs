using Gateway.Bussiness;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetAccountsController : ControllerBase
    {
        private readonly ILogger<GetAccountsController> _logger;

        public GetAccountsController(ILogger<GetAccountsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<AccountResponse>> Get()
        {
            return await HyperledgerBusiness.GetAccounts("");
            
        }
    }
}
