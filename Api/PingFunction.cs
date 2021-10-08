using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TheSwamp.Api.Interfaces;
using TheSwamp.Shared;

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
        public ActionResult<string> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var x = _myService.Boosh();
            log.LogInformation(x);

            return x;// new OkObjectResult(x);
        }

        [FunctionName("devices")]
        public async Task<ActionResult<DataSource[]>> Devices(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var x = await _devices.GetDevicesAsync();
            return x;
        }
    }
}
