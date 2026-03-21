using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace TheSwamp.Api
{
    public class HubFunction
    {
        [Function("hub-negotiate")]
        public IActionResult Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, Route = "hub/negotiate")] HttpRequest req,
            [SignalRConnectionInfoInput(HubName = "theswamp")] SignalRConnectionInfo connectionInfo)
        {
            return new OkObjectResult(connectionInfo);
        }
    }
}

