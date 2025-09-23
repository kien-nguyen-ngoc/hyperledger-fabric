using Gateway.Commands;
using Gateway.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreateAccountController : ControllerBase
    {
        private readonly ILogger<CreateAccountController> _logger;

        public CreateAccountController(ILogger<CreateAccountController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task Post(string id)
        {
            await HyperledgerCommander.Send("", new CreateAccountCommand { Id = id});
        }
    }
}
