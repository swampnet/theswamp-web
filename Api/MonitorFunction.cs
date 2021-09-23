using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;
using TheSwamp.Api.Interfaces;

namespace TheSwamp.Api
{
    public class MonitorFunction
    {
        private readonly IAuth _auth;
        private readonly IMonitor _monitor;

        public MonitorFunction(IAuth auth, IMonitor monitor)
        {
            _auth = auth;
            _monitor = monitor;
        }

        [FunctionName("monitor-list")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "monitor/{id}")] HttpRequest req,
            int id,
            ILogger log)
        {
            var x = await _monitor.GetValuesAsync(id);

            return new OkObjectResult(x);
        }


        [FunctionName("monitor-post")]
        public IActionResult Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "monitor/{id}")] HttpRequest req,
            int id,
            ILogger log)
        {
            if (!_auth.Authenticate(req))
            {
                return new UnauthorizedResult();
            }

            return new OkObjectResult($"{id} - {DateTime.Now}");
        }
    }
}
