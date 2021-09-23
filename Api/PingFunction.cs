using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TheSwamp.Api.Interfaces;

namespace TheSwamp.Api
{
    public class PingFunction
    {
        private readonly IConfiguration _cfg;
        private readonly IMyService _myService;
        private readonly IMonitor _devices;

        public PingFunction(IConfiguration cfg, IMyService myService, IMonitor devices)
        {
            _cfg = cfg;
            _myService = myService;
            _devices = devices;
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

        [FunctionName("devices")]
        public async Task<IActionResult> Devices(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var x = await _devices.GetDevicesAsync();

            return new OkObjectResult(x);
        }
    }
}
