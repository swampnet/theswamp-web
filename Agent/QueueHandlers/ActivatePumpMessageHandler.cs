using Iot.Device.Max7219;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Device.Spi;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent.QueueHandlers
{
    internal class ActivatePumpMessageHandler : IQueueHandler
    {
        private readonly IConfiguration _cfg;
        private readonly Dictionary<int, DateTime> _lastActivation = new Dictionary<int, DateTime>();

        public ActivatePumpMessageHandler(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        public bool CanProcess(AgentMessage msg)
        {
            return msg.Type.EqualsNoCase("activate-pump");
        }

        public async Task ProcessAsync(AgentMessage msg)
        {
            var channel = msg.Properties.IntValue("channel", -1);
            if(channel == -1)
            {
                throw new ArgumentException("Missing channel");
            }

            DateTime lastActive;
            if(!_lastActivation.TryGetValue(channel, out lastActive))
            {
                lastActive = DateTime.MinValue;
            }

            // @TODO: From cfg
            var cooldown = TimeSpan.FromMinutes(1);

            var x = DateTime.UtcNow - lastActive;

            if (x > cooldown)
            {
                // Activate pump for <time> seconds.
                // Note that each pump (ie, channel) might need a different time (longer hoses, different pump types etc)
                var d = 1000;
                Console.WriteLine($"@TODO: Activate pump {channel} - {d}ms");
                await Task.Delay(d);
                Console.WriteLine($"@TODO: Deactivate pump {channel}!");

                _lastActivation[channel] = DateTime.UtcNow;
            }
            else
            {
                Console.WriteLine($"Throttled pump {channel} (cooldown {cooldown-x})");
            }
        }
    }
}
