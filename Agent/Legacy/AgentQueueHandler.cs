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
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(AgentMessage msg)
        {
            Message = msg;
        }

        public AgentMessage Message { get; private set; }
    }


    internal class AgentQueueHandler
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        private readonly IConfiguration _cfg;
        private readonly LedMatrixMessageHandler _handler;

        public AgentQueueHandler(IConfiguration cfg)
        {
            _cfg = cfg;
            _handler = new LedMatrixMessageHandler(_cfg);
        }

        public async Task StartAsync()
        {
            var servicebus = new ServiceBusClient(_cfg["azure.servicebus"]);
            var servicebusProcessor = servicebus.CreateProcessor("iot_agent");
            servicebusProcessor.ProcessMessageAsync += ProcessMessageAsync;
            servicebusProcessor.ProcessErrorAsync += ProcessErrorAsync;
            await servicebusProcessor.StartProcessingAsync();
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
            Console.WriteLine($"Message Q:{arg.Message.MessageId} received");
            await arg.CompleteMessageAsync(arg.Message);

            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(msg));

            try
            {
                // @TODO: Find right handler(s) for msg.Type
                await _handler.ProcessAsync(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
