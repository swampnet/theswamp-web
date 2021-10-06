using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
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
                msg.Properties.Add(new Property("client-ip", req.GetClientIp()));
                await LogMessageAsync(msg);

                await using (ServiceBusClient client = new ServiceBusClient(_cfg["azure.servicebus"]))
                {
                    var sender = client.CreateSender("iot_agent");
                    var message = new ServiceBusMessage(json);
                    await sender.SendMessageAsync(message);
                }
            }

            return new OkResult();
        }


        [FunctionName("agent-recent-messages")]
        public async Task<IActionResult> GetRecent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "agent/queue/{type}")] HttpRequest req, 
            string type)        
        {
            var messages = await _iotContext.Messages
                .Include(f => f.Properties)
                .Where(f => f.Type == type)
                .OrderByDescending(f => f.TimestampUtc)
                .Take(10)
                .Select(m => new AgentMessage() { 
                    Type = m.Type,
                    TimestampUtc = m.TimestampUtc,
                    Properties = m.Properties.Select(x => new Property()
                    {
                        Name = x.Name,
                        Value = x.Value
                    }).ToList()
                })
                .ToArrayAsync();

            return new OkObjectResult(messages);
        }



        private async Task LogMessageAsync(AgentMessage msg)
        {
            var m = new Message()
            {
                Type = msg.Type
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

            _log.LogInformation($"post from {msg.Properties.StringValue("ClientIp", "unknown")}");
        }
    }
}
