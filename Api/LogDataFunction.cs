using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using TheSwamp.Api.Interfaces;
using TheSwamp.Shared;

namespace TheSwamp.Api
{
    /// <summary>
    /// Log / search data
    /// </summary>
    public class LogDataFunction
    {
        private readonly IAuth _auth;
        private readonly IDataLogger _monitor;

        public LogDataFunction(IAuth auth, IDataLogger monitor)
        {
            _auth = auth;
            _monitor = monitor;
        }


        [FunctionName("GET-log-data-device")]
        public async Task<IActionResult> GetDevice(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "log/data/device")] HttpRequest req,
            ILogger log)
        {
            if (!await _auth.AuthenticateAsync(req))
            {
                return new UnauthorizedResult();
            }

            var name = req.Query["name"];

            log.LogInformation($"Get device: {name}");

            var x = await _monitor.GetDeviceAsync(req.Query["name"]);

            return new OkObjectResult(x);
        }


        [FunctionName("GET-log-data")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "log/data")] HttpRequest req,
            ILogger log)
        {
            log.LogDebug("Get summary");

            var x = await _monitor.GetDataSourceSummaryAsync();

            return new OkObjectResult(x);
        }


        [FunctionName("GET-log-data-history")]
        public async Task<ActionResult<DataSourceSummary>> GetHistory(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "log/data/{device}")] HttpRequest req,
            string device,
            ILogger log)
        {
            log.LogDebug($"Get history for {device}");

            var x = await _monitor.GetHistory(device);

            return x;
        }


        [FunctionName("POST-log-data")]
        public async Task<IActionResult> PostValues(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "log/data")] HttpRequest req,
            [SignalR(HubName = "theswamp")] IAsyncCollector<SignalRMessage> signalRMessages,
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

            var x = await _monitor.GetDataSourceSummaryAsync();
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "monitor-values",
                    Arguments = new[] { x }
                });

            return new OkResult();
        }

    }

}
