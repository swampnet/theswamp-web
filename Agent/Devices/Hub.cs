using Iot.Device.SensorHub;
using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent.Devices
{
    class Hub : DeviceBase
    {
        private const int _busId = 1;
        private SensorHub _sh;

        internal override void Initialise(IEnumerable<IProperty> cfg)
        {
            // 0x17
            _sh = new SensorHub(I2cDevice.Create(new(_busId, SensorHub.DefaultI2cAddress)));
        }


        public override Task<double?> ReadAsync(IEnumerable<IProperty> parameters)
        {
            double? val = null;
            var sensor = parameters.StringValue("sensor");
            
            switch (sensor)
            {
                case "OffBoardTemperature":
                    if (_sh.TryReadOffBoardTemperature(out var t))
                    {
                        val = t.DegreesCelsius;
                    }
                    break;

                case "ReadBarometerPressure":
                    if (_sh.TryReadBarometerPressure(out var b))
                    {
                        val = b.Bars;
                    }
                    break;

                case "ReadBarometerTemperature":
                    if (_sh.TryReadBarometerTemperature(out var bt))
                    {
                        val = bt.DegreesCelsius;
                    }
                    break;

                case "ReadIlluminance":
                    if (_sh.TryReadIlluminance(out var l))
                    {
                        val = l.Lux;
                    }
                    break;

                case "ReadOnBoardTemperature":
                    if (_sh.TryReadOnBoardTemperature(out var ot))
                    {
                        val = ot.DegreesCelsius;
                    }
                    break;

                case "ReadRelativeHumidity":
                    if (_sh.TryReadRelativeHumidity(out var h))
                    {
                        val = h.Percent;
                    }
                    break;

                case "MotionDetected":
                    val = _sh.IsMotionDetected ? 1 : 0;
                    break;

                default:
                    throw new InvalidOperationException($"Unknown sensor: '{sensor}' (case sensitive)");
            }

            return Task.FromResult(val);
        }
    }
}
