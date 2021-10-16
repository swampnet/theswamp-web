using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using TheSwamp.Api.Interfaces;
using TheSwamp.Shared;

namespace TheSwamp.Api
{
    public class TimerFunction
    {
        private readonly IPostMessage _postMessage;

        public TimerFunction(IPostMessage postMessage)
        {
            _postMessage = postMessage;
        }


        [FunctionName("TimerFunction")]
        public async Task Run([TimerTrigger("0 0 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await _postMessage.PostAsync(new AgentMessage()
            {
                Type = "led-matrix",
                Properties = new List<Property>()
                {
                    new Property()
                    {
                        Name = "content",
                        Value = $"{DateTime.Now}"
                    }
                }
            });
        }
    }
}
