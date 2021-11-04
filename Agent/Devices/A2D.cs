using Iot.Device.Adc;
using System;
using System.Collections.Generic;
using System.Device.Spi;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent.Devices
{
    class A2D : DeviceBase
    {
        private SpiConnectionSettings _hardwareSpiSettings;
        private SpiDevice _spi;
        private Mcp3008 _mcp;
        private int _busId;
        private int _chipSelectLine;

        internal override void Initialise(IEnumerable<IProperty> cfg)
        {
            _chipSelectLine = cfg.IntValue("chip-select");
            _busId = cfg.IntValue("bus-id");
            _hardwareSpiSettings = new SpiConnectionSettings(_busId, _chipSelectLine)
            {
                ClockFrequency = 1000000 // from cfg?
            };

            _spi = SpiDevice.Create(_hardwareSpiSettings);
            _mcp = new Mcp3008(_spi);
        }


        public override Task<double?> ReadAsync(IEnumerable<IProperty> parameters)
        {
            double? val = null;
            var channel = parameters.IntValue("channel");

            val = _mcp.Read(channel);

            // The chip is 10-bit, which means that it will generate values from
            // 0-1023 (2^10 is 1024).
            // We can transform the value to a more familiar 0-100 scale by dividing
            // the 10-bit value by 10.24.
            val = val / 10.24;
            val = Math.Round(val.Value);
            return Task.FromResult(val);
        }
    }
}
