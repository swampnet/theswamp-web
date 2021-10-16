using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Api.DAL.IOT;
using TheSwamp.Api.DAL.IOT.Entities;
using TheSwamp.Api.Interfaces;
using TheSwamp.Shared;

namespace TheSwamp.Api.Services
{
    internal class PostMessageService : IPostMessage
    {
        private readonly ILogger<PostMessageService> _log;
        private readonly IConfiguration _cfg;
        private readonly IotContext _iotContext;

        public PostMessageService(ILogger<PostMessageService> log, IConfiguration cfg, IotContext iotContext)
        {
            _log = log;
            _cfg = cfg;
            _iotContext = iotContext;
        }


        public async Task PostAsync(AgentMessage msg)
        {
            await LogMessageAsync(msg);

            await using (ServiceBusClient client = new ServiceBusClient(_cfg["azure.servicebus"]))
            {
                var sender = client.CreateSender("iot_agent");
                var message = new ServiceBusMessage(JsonConvert.SerializeObject(msg));
                await sender.SendMessageAsync(message);
            }
        }

        private async Task LogMessageAsync(AgentMessage msg)
        {
            var m = new Message()
            {
                Type = msg.Type
            };

            if (msg.Properties != null)
            {
                foreach (var p in msg.Properties)
                {
                    m.Properties.Add(new MessageProperty()
                    {
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
