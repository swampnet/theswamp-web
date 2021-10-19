using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace TheSwamp.Api
{
    public class HubFunction : ServerlessHub
    {
        [FunctionName("hub-negotiate")]
        public SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, Route = "hub/negotiate")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "theswamp")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
    }
}
