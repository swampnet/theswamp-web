using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using TheSwamp.Api.Interfaces;

namespace TheSwamp.Api
{
    public class PingFunction
    {
        private readonly IMyService _myService;

        public PingFunction(IMyService myService)
        {
            _myService = myService;
        }

        [FunctionName("ping")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var x = _myService.Boosh();
            log.LogInformation(x);

            return new OkObjectResult(x);
        }
    }
}
