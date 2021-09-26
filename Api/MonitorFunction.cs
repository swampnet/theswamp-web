using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using TheSwamp.Api.Interfaces;
using TheSwamp.Shared;

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


        [FunctionName("monitor-get-datasource")]
        public async Task<IActionResult> GetDevice(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "monitor")] HttpRequest req,
            ILogger log)
        {
            if (!await _auth.AuthenticateAsync(req))
            {
                return new UnauthorizedResult();
            }

            var x = await _monitor.GetDeviceAsync(req.Query["dataSource"]);

            return new OkObjectResult(x);
        }

        [FunctionName("monitor-list")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "monitor/summary")] HttpRequest req,
            ILogger log)
        {
            var x = await _monitor.GetDataSourceSummaryAsync();

            return new OkObjectResult(x);
        }

        [FunctionName("monitor-post-values")]
        public async Task<IActionResult> PostValues(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "monitor")] HttpRequest req,
            ILogger log)
        {
            if (!await _auth.AuthenticateAsync(req))
            {
                return new UnauthorizedResult();
            }

            using (var reader = new StreamReader(req.Body))
            {
                var json = await reader.ReadToEndAsync();
                
                await _monitor.PostValuesAsync(JsonConvert.DeserializeObject<DataPoint[]>(json));
            }

            return new OkResult();
        }


        [FunctionName("monitor-post")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "monitor/{id}")] HttpRequest req,
            int id,
            ILogger log)
        {
            if (!await _auth.AuthenticateAsync(req))
            {
                return new UnauthorizedResult();
            }

            return new OkObjectResult($"{id} - {DateTime.Now}");
        }
    }
}
