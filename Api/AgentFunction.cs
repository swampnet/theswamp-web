﻿using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheSwamp.Api.DAL.IOT;
using TheSwamp.Api.DAL.IOT.Entities;
using TheSwamp.Api.Interfaces;
using TheSwamp.Shared;

namespace TheSwamp.Api
{
    public class AgentFunction
    {
        private readonly IConfiguration _cfg;
        private readonly IotContext _iotContext;
        private readonly ILogger<AgentFunction> _log;
        private readonly IPostMessage _postMessage;

        public AgentFunction(ILogger<AgentFunction> log, IPostMessage postMessage, IConfiguration cfg, IotContext iotContext)
        {
            _log = log;
            _postMessage = postMessage;
            _cfg = cfg;
            _iotContext = iotContext;
        }

        [FunctionName("agent-post-message")]
        public async Task<IActionResult> PostMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "agent/queue")] HttpRequest req,
            [SignalR(HubName = "serverlessSample")] IAsyncCollector<SignalRMessage> signalRMessages
            )
        {
            using (var reader = new StreamReader(req.Body))
            {
                var json = await reader.ReadToEndAsync();
                var msg = JsonConvert.DeserializeObject<AgentMessage>(json);
                msg.Properties.Add(new Property("client-ip", req.GetClientIp()));
                await _postMessage.PostAsync(msg);

                await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "led-message",
                        Arguments = new[] { msg }
                    });
            }

            return new OkResult();
        }


        [FunctionName("agent-recent-messages")]
        public async Task<ActionResult<AgentMessage[]>> GetRecent(
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

            return messages;
        }
    }
}
