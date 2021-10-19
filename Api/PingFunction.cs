using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TheSwamp.Api.Interfaces;
using TheSwamp.Shared;

namespace TheSwamp.Api
{
    public class PingFunction
    {
        private readonly IConfiguration _cfg;

        public PingFunction(IConfiguration cfg)
        {
            _cfg = cfg;
        }


        [FunctionName("ping")]
        public async Task<ActionResult<string>> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            await Task.CompletedTask;

            log.LogInformation("pong");

            return new OkObjectResult($"pong @ {DateTime.Now}");
        }
    }
}
