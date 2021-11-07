using Iot.Device.CpuTemperature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent.Devices
{
    class Cpu : DeviceBase
    {
        private CpuTemperature _cpuTemp = new CpuTemperature();

        public override Task<double?> ReadAsync(IEnumerable<IProperty> parameters)
        {
            double? val = null;
            
            //"temp:cpu"

            var sensor = parameters.StringValue("sensor");
            var parts = sensor.Split(':', StringSplitOptions.RemoveEmptyEntries);

            switch (parts[0].ToLowerInvariant())
            {
                case "temp":
                    if(parts.Count() != 2)
                    {
                        throw new InvalidOperationException($"Invalid temp format: 'temp:<sensor>'");
                    }

                    if (_cpuTemp.IsAvailable)
                    {
                        var temperatures = _cpuTemp.ReadTemperatures();

                        Console.WriteLine("CPU Temperature\n" + string.Join("\n", temperatures.Select(t => $"[{t.Sensor}] {t.Temperature}")));
                        Console.WriteLine($"Looking for: '{parts[1]}'");
                        var x = temperatures.SingleOrDefault(t => t.Sensor.EqualsNoCase(parts[1]));

                        if (!double.IsNaN(x.Temperature.DegreesCelsius))
                        {
                            val = x.Temperature.DegreesCelsius;
                        }
                        else
                        {
                            Console.WriteLine("Unable to read Temperature.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"CPU temperature is not available");
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Unknown operation: '{parts[0]}' (From '{sensor}')");
            }

            return Task.FromResult(val);
        }
    }
}
