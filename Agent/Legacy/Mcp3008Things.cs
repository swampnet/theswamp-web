using Iot.Device.Adc;
using System;
using System.Device.Spi;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent
{
    class Mcp3008Things : ISampleProvider
    {
        private const int _chipSelectLine = 1;
        private const int _busId = 0;
        private readonly SpiConnectionSettings _hardwareSpiSettings;
        private readonly SpiDevice _spi;
        private readonly Mcp3008 _mcp;

        // basically which pin we read from
        readonly int _channel_pot = 0;
        readonly int _channel_moisture_01 = 1;
        readonly int _channel_moisture_02 = 2;
        readonly int _channel_moisture_03 = 3;
        readonly int _channel_moisture_04 = 4;

        public TimeSpan PollInterval => TimeSpan.FromMilliseconds(500);

        public Mcp3008Things()
        {
            _hardwareSpiSettings = new SpiConnectionSettings(_busId, _chipSelectLine)
            {
                ClockFrequency = 1000000
            };

            _spi = SpiDevice.Create(_hardwareSpiSettings);
            _mcp = new Mcp3008(_spi);
        }


        public async Task PollAsync(Monitor monitor)
        {
            await monitor.AddDataPointAsync("POT", Read(_channel_pot));
            await monitor.AddDataPointAsync("Moisture 01", Read(_channel_moisture_01));
            await monitor.AddDataPointAsync("Moisture 02", Read(_channel_moisture_02));
            await monitor.AddDataPointAsync("Moisture 03", Read(_channel_moisture_03));
            await monitor.AddDataPointAsync("Moisture 04", Read(_channel_moisture_04));
        }


        private double Read(int channel)
        {
            double value = _mcp.Read(channel);

            // The chip is 10-bit, which means that it will generate values from
            // 0-1023 (2^10 is 1024).
            // We can transform the value to a more familiar 0-100 scale by dividing
            // the 10-bit value by 10.24.
            value = value / 10.24;
            value = Math.Round(value);
            return value;
        }
    }
}
