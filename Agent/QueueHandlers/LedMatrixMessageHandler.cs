using Iot.Device.Max7219;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Device.Spi;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent.QueueHandlers
{
    internal class LedMatrixMessageHandler : IQueueHandler
    {
        private const int _chipSelectLine = 0;
        private const int _busId = 0;
        private readonly SpiConnectionSettings _hardwareSpiSettings;
        private readonly IConfiguration _cfg;
        private Max7219 _max7219;
        private SpiDevice _spi;

        public LedMatrixMessageHandler(IConfiguration cfg)
        {
            _cfg = cfg;
            _hardwareSpiSettings = new SpiConnectionSettings(_busId, _chipSelectLine)
            {
                ClockFrequency = 10_000_000,
                Mode = SpiMode.Mode0
            };
        }


        #region IQueueHandler
        public bool CanProcess(AgentMessage msg)
        {
            return msg.Type.EqualsNoCase("led-message");
        }

        public Task ProcessAsync(AgentMessage msg)
        {
            if(_spi == null)
            {
                _spi = SpiDevice.Create(_hardwareSpiSettings);
                _max7219 = new Max7219(_spi, cascadedDevices: 4);
                _max7219.Init();
            }

            var content = msg.Properties.StringValue("content", "No content....");
            Console.WriteLine($"{this.GetType().Name} '{content}'");
            _max7219.Rotation = RotationType.Left;
            var writer = new MatrixGraphics(_max7219, Fonts.CP437);

            writer.Font = Fonts.CP437;
            writer.ShowMessage(content, alwaysScroll: true);

            return Task.CompletedTask;
        }

        #endregion
    }
}
