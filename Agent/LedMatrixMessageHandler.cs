using Iot.Device.Max7219;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Device.Spi;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent
{
    internal class LedMatrixMessageHandler
    {
        private readonly IConfiguration _cfg;
        private readonly SpiDevice _spiDevice;
        private readonly Max7219 _devices;

        public LedMatrixMessageHandler(IConfiguration cfg)
        {
            _cfg = cfg;
            var connectionSettings = new SpiConnectionSettings(0, 0)
            {
                ClockFrequency = 10_000_000,
                Mode = SpiMode.Mode0
            };
            _spiDevice = SpiDevice.Create(connectionSettings);
            _devices = new Max7219(_spiDevice, cascadedDevices: 4);
            _devices.Init();
        }

        public Task ProcessAsync(AgentMessage msg)
        {
            var content = msg.Properties.StringValue("content", "No content....");
            Console.WriteLine($"{this.GetType().Name} '{content}'");
            _devices.Rotation = RotationType.Left;
            var writer = new MatrixGraphics(_devices, Fonts.CP437);

            writer.Font = Fonts.CP437;
            writer.ShowMessage(content, alwaysScroll: true);

            return Task.CompletedTask;
        }
    }
}
