using System;
using System.Threading.Tasks;
using TheSwamp.Shared;
using Iot.Device.CpuTemperature;

namespace Agent
{
    public class CpuTemperatureThing : IThing
    {
        public TimeSpan PollInterval => TimeSpan.FromMinutes(5);
        private CpuTemperature _cpuTemp = new CpuTemperature();

        public async Task PollAsync(Monitor monitor)
        {
            if (_cpuTemp.IsAvailable)
            {
                var temperature = _cpuTemp.ReadTemperatures();
                foreach (var entry in temperature)
                {
                    if (!double.IsNaN(entry.Temperature.DegreesCelsius))
                    {
                        await monitor.AddDataPointAsync($"temp:{entry.Sensor}".ToLowerInvariant(), entry.Temperature.DegreesCelsius);
                    }
                    else
                    {
                        Console.WriteLine("Unable to read Temperature.");
                    }
                }
            }
            else
            {
                Console.WriteLine($"CPU temperature is not available");
            }
        }
    }
}
