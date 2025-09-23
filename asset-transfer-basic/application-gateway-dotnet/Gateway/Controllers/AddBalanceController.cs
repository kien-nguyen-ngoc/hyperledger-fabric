using Gateway.Commands;
using Gateway.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddBalanceController : ControllerBase
    {
        private readonly ILogger<AddBalanceController> _logger;

        public AddBalanceController(ILogger<AddBalanceController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task Post(string id, string amount)
        {
            await HyperledgerCommander.Send("", new AddBalanceCommand { Id = id, Amount = amount });
        }
    }
}
