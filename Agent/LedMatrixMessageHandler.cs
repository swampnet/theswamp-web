using Iot.Device.Max7219;
using Microsoft.Extensions.Configuration;
using System;
using System.Device.Spi;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent
{
    internal class LedMatrixMessageHandler
    {
        private const int _chipSelectLine = 0;
        private const int _busId = 0;
        private readonly SpiConnectionSettings _hardwareSpiSettings;
        private readonly SpiDevice _spi;


        private readonly IConfiguration _cfg;
        private readonly Max7219 _devices;

        public LedMatrixMessageHandler(IConfiguration cfg)
        {
            _cfg = cfg;
            _hardwareSpiSettings = new SpiConnectionSettings(_busId, _chipSelectLine)
            {
                ClockFrequency = 10_000_000,
                Mode = SpiMode.Mode0
            };
            _spi = SpiDevice.Create(_hardwareSpiSettings);
            _devices = new Max7219(_spi, cascadedDevices: 4);
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
