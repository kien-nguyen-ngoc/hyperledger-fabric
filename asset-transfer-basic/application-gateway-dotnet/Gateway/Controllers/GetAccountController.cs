using Gateway.Bussiness;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetAccountController : ControllerBase
    {
        private readonly ILogger<GetAccountsController> _logger;

        public GetAccountController(ILogger<GetAccountsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<AccountResponse> Get(string id)
        {
            return await HyperledgerBusiness.GetAccount("", id);
            
        }
    }
}
