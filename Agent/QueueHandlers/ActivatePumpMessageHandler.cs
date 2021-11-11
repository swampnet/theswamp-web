using Iot.Device.Max7219;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Spi;
using System.Linq;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent.QueueHandlers
{
    internal class ActivatePumpMessageHandler : IQueueHandler
    {
        private readonly IConfiguration _cfg;
        private readonly Dictionary<int, DateTime> _lastActivation = new Dictionary<int, DateTime>();
        private GpioController _gpio;

        private IEnumerable<PumpConfig> Pumps => _cfg.GetSection("pumps").Get<PumpConfig[]>();

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

            var pump = Pumps.SingleOrDefault(p=>p.Channel == channel);
            if(pump == null)
            {
                throw new NullReferenceException($"Pump channel {channel} undefined");
            }

            if(_gpio == null)
            {
                _gpio = new GpioController(PinNumberingScheme.Board);
            }

            DateTime lastActive;
            if(!_lastActivation.TryGetValue(pump.Channel, out lastActive))
            {
                lastActive = DateTime.MinValue;
                _gpio.OpenPin(pump.Channel, PinMode.Output, PinValue.High);
            }

            var x = DateTime.UtcNow - lastActive;

            if (x > pump.Cooldown)
            {
                Console.WriteLine($"Activate pump {pump.Channel} - {pump.SquirtSeconds:0.0}s");
                _gpio.Write(pump.Channel, PinValue.Low);
                await Task.Delay((int)(pump.SquirtSeconds * 1000.0));

                Console.WriteLine($"Deactivate pump {pump.Channel}!");
                _gpio.Write(pump.Channel, PinValue.High);

                _lastActivation[pump.Channel] = DateTime.UtcNow;
            }
            else
            {
                Console.WriteLine($"Throttled pump {pump.Channel} (cooldown {pump.Cooldown-x})");
            }
        }        
    }
}
