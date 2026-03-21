using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
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
        private readonly ILogger<LogDataFunction> _log;

        public LogDataFunction(IAuth auth, IDataLogger monitor, ILogger<LogDataFunction> log)
        {
            _auth = auth;
            _monitor = monitor;
            _log = log;
        }


        [Function("GET-log-data-device")]
        public async Task<IActionResult> GetDevice(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "log/data/device")] HttpRequest req)
        {
            if (!await _auth.AuthenticateAsync(req))
            {
                return new UnauthorizedResult();
            }

            var name = req.Query["name"];

            _log.LogInformation($"Get device: {name}");

            var x = await _monitor.GetDeviceAsync(req.Query["name"]);

            return new OkObjectResult(x);
        }


        [Function("GET-log-data")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "log/data")] HttpRequest req)
        {
            _log.LogDebug("Get summary");

            var x = await _monitor.GetDataSourceSummaryAsync();

            return new OkObjectResult(x);
        }


        [Function("GET-log-data-history")]
        public async Task<ActionResult<DataSourceDetails>> GetHistory(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "log/data/{device}")] HttpRequest req,
            string device)
        {
            _log.LogDebug($"Get history for {device}");

            var x = await _monitor.GetHistory(device);

            return x;
        }


        [Function("POST-log-data")]
        public async Task<PostLogDataOutput> PostValues(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "log/data")] HttpRequest req)
        {
            if (!await _auth.AuthenticateAsync(req))
            {
                return new PostLogDataOutput { HttpResponse = new UnauthorizedResult() };
            }

            using (var reader = new StreamReader(req.Body))
            {
                var json = await reader.ReadToEndAsync();

                await _monitor.PostValuesAsync(JsonConvert.DeserializeObject<DataPoint[]>(json));
            }

            var x = await _monitor.GetDataSourceSummaryAsync();

            return new PostLogDataOutput
            {
                Message = new SignalRMessageAction("monitor-values", new object[] { x }),
                HttpResponse = new OkResult()
            };
        }

    }

    public class PostLogDataOutput
    {
        [SignalROutput(HubName = "theswamp")]
        public SignalRMessageAction? Message { get; set; }

        [HttpResult]
        public IActionResult? HttpResponse { get; set; }
    }

}

