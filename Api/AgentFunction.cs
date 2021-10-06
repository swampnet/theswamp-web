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
using TheSwamp.Api.DAL.IOT;
using TheSwamp.Api.DAL.IOT.Entities;
using TheSwamp.Shared;

namespace TheSwamp.Api
{
    public class AgentFunction
    {
        private readonly IConfiguration _cfg;
        private readonly IotContext _iotContext;
        private readonly ILogger<AgentFunction> _log;

        public AgentFunction(ILogger<AgentFunction> log, IConfiguration cfg, IotContext iotContext)
        {
            _log = log;
            _cfg = cfg;
            _iotContext = iotContext;
        }

        [FunctionName("agent-post-message")]
        public async Task<IActionResult> PostMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "agent/queue")] HttpRequest req)
        {
            using (var reader = new StreamReader(req.Body))
            {
                var json = await reader.ReadToEndAsync();
                var msg = JsonConvert.DeserializeObject<AgentMessage>(json);

                await LogMessageAsync(req.GetClientIp(), msg);

                await using (ServiceBusClient client = new ServiceBusClient(_cfg["azure.servicebus"]))
                {
                    var sender = client.CreateSender("iot_agent");
                    var message = new ServiceBusMessage(json);
                    await sender.SendMessageAsync(message);
                }
            }

            return new OkResult();
        }



        private async Task LogMessageAsync(string clientIp, AgentMessage msg)
        {
            var m = new Message()
            {
                Type = msg.Type,
                ClientIp = clientIp
            };

            if(msg.Properties != null)
            {
                foreach(var p in msg.Properties)
                {
                    m.Properties.Add(new MessageProperty() { 
                        Name = p.Name,
                        Value = p.Value
                    });
                }
            }

            await _iotContext.Messages.AddAsync(m);
            await _iotContext.SaveChangesAsync();

            _log.LogInformation($"post from {clientIp}");
        }
    }
}
