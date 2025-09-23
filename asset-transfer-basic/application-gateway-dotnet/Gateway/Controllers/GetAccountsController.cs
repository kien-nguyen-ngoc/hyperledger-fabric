using Gateway.Commands;
using Gateway.Utils;
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
        public async Task<string> Get()
        {
            string s = await HyperledgerCommander.Receive("", new GetAllAccountsCommand());
            _logger.LogInformation(s);
            return s;
        }
    }
}
