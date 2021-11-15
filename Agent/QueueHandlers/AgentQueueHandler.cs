using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent
{
    internal class AgentQueueHandler
    {
        private readonly IConfiguration _cfg;
        private readonly IEnumerable<IQueueHandler> _queueHandlers;

        public AgentQueueHandler(IConfiguration cfg, IEnumerable<IQueueHandler> queueHandlers)
        {
            _cfg = cfg;
            _queueHandlers = queueHandlers;
        }

        public async Task StartAsync()
        {
            var servicebus = new ServiceBusClient(_cfg["azure.servicebus"]);
            var servicebusProcessor = servicebus.CreateProcessor("iot_agent", new ServiceBusProcessorOptions() { 
                PrefetchCount = 0,
                MaxConcurrentCalls = 1                
            });
            servicebusProcessor.ProcessMessageAsync += ProcessMessageAsync;
            servicebusProcessor.ProcessErrorAsync += ProcessErrorAsync;
            await servicebusProcessor.StartProcessingAsync();

            Console.WriteLine($"{GetType().Name} Start ({_queueHandlers.Count()} handlers)");
        }


        private Task ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception);
            return Task.CompletedTask;
        }


        private async Task ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            string json = arg.Message.Body.ToString();
            var msg = JsonConvert.DeserializeObject<AgentMessage>(json);
            Console.WriteLine($"[{msg.Type}] Q:{arg.Message.MessageId} received");
            await arg.CompleteMessageAsync(arg.Message);

            foreach (var handler in _queueHandlers.Where(h => h.CanProcess(msg)))
            {
                try
                {
                    Console.WriteLine($"{handler.GetType().Name} Processing message {msg.Type}");
                    await handler.ProcessAsync(msg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Handler {handler.GetType().Name} failed for message: {msg.Type}:\n"+ex);
                }
            }
        }
    }
}
