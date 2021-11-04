using Iot.Device.SensorHub;
using System;
using System.Device.I2c;
using System.Device.Spi;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent
{
    public class SensorHubThing : ISampleProvider
    {
        private const int _busId = 1;

        private readonly SensorHub _sh;

        public TimeSpan PollInterval => TimeSpan.FromSeconds(60);

        public SensorHubThing()
        {
            // 0x17
            _sh = new SensorHub(I2cDevice.Create(new(_busId, SensorHub.DefaultI2cAddress)));
        }

        public async Task PollAsync(Monitor monitor)
        {
            if (_sh.TryReadOffBoardTemperature(out var t))
            {
                await monitor.AddDataPointAsync("OffBoard temperature", t.DegreesCelsius);
            }

            if (_sh.TryReadBarometerPressure(out var p))
            {
                await monitor.AddDataPointAsync("Pressure", p.Bars);
            }

            if (_sh.TryReadBarometerTemperature(out var bt))
            {
                await monitor.AddDataPointAsync("Barometer temperature", bt.DegreesCelsius);
            }

            if (_sh.TryReadIlluminance(out var l))
            {
                await monitor.AddDataPointAsync("Illuminance", l.Lux);
            }

            if (_sh.TryReadOnBoardTemperature(out var ot))
            {
                await monitor.AddDataPointAsync("OnBoard temperature", ot.DegreesCelsius);
            }

            if (_sh.TryReadRelativeHumidity(out var h))
            {
                await monitor.AddDataPointAsync("Relative humidity", h.Percent);
            }
        }
    }
}
