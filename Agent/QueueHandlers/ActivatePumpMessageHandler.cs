using Iot.Device.Max7219;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Spi;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent.QueueHandlers
{
    internal class ActivatePumpMessageHandler : IQueueHandler
    {
        private readonly IConfiguration _cfg;
        private readonly Dictionary<int, DateTime> _lastActivation = new Dictionary<int, DateTime>();
        private GpioController _gpio;

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

            if(_gpio == null)
            {
                _gpio = new GpioController(PinNumberingScheme.Board);
            }

            DateTime lastActive;
            if(!_lastActivation.TryGetValue(channel, out lastActive))
            {
                lastActive = DateTime.MinValue;
                _gpio.OpenPin(channel, PinMode.Output, PinValue.High);
            }

            // @TODO: From cfg
            var cooldown = TimeSpan.FromSeconds(10);

            var x = DateTime.UtcNow - lastActive;

            if (x > cooldown)
            {
                // Activate pump for <time> seconds.
                // Note that each pump (ie, channel) might need a different time (longer hoses, different pump types etc)
                var d = 1000;
                Console.WriteLine($"Activate pump {channel} - {d}ms");
                _gpio.Write(channel, PinValue.Low);
                await Task.Delay(d);

                Console.WriteLine($"Deactivate pump {channel}!");
                _gpio.Write(channel, PinValue.High);

                _lastActivation[channel] = DateTime.UtcNow;
            }
            else
            {
                Console.WriteLine($"Throttled pump {channel} (cooldown {cooldown-x})");
            }
        }
    }
}
