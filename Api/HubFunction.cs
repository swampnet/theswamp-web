using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace TheSwamp.Api
{
    public class HubFunction : ServerlessHub
    {
        [FunctionName("negotiate")]
        public SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
            [SignalRConnectionInfo(HubName = "serverlessSample")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }


        //[FunctionName("broadcast")]
        //public async Task Broadcast(
        //    [TimerTrigger("0 * * * * *")] TimerInfo myTimer,
        //    [SignalR(HubName = "serverlessSample")] IAsyncCollector<SignalRMessage> signalRMessages)
        //{
        //    await signalRMessages.AddAsync(
        //        new SignalRMessage
        //        {
        //            Target = "tick",
        //            Arguments = new[] { $"{DateTime.Now}" }
        //        });
        //}
    }
}
