using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace TheSwamp.Api
{
    public class AgentFunction
    {
        private readonly IConfiguration _cfg;

        public AgentFunction(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        [FunctionName("agent-post-message")]
        public async Task<IActionResult> PostMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            using (var reader = new StreamReader(req.Body))
            {
                var json = await reader.ReadToEndAsync();
                //var msg = JsonConvert.DeserializeObject<AgentMessage>(json);

                await using(ServiceBusClient client = new ServiceBusClient(_cfg["azure.servicebus"]))
                {
                    var sender = client.CreateSender("iot_agent");
                    var message = new ServiceBusMessage(json);
                    await sender.SendMessageAsync(message);
                }
            }

            return new OkResult();
        }
    }
}
