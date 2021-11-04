using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent
{
    class SampleDefinition
    {
        public string Name { get; set; }
        public string Device { get; set; }
        public TimeSpan Frequency { get; set; }
        public double? MinChange { get; set; }
        public Property[] Cfg { get; set; }
        public DateTime? LastSampleOn { get; set; }
        public bool IsInProgress { get; internal set; } = false;
        public double? LastValue { get; internal set; }
    }

    
    class SamplesService : ISamples
    {
        private readonly IConfiguration _cfg;
        private readonly IServiceProvider _serviceProvider;
        private IEnumerable<IDevice> _devices;
        private IEnumerable<SampleDefinition> _sampleDefinitions;

        public SamplesService(IConfiguration cfg, IServiceProvider serviceProvider)
        {
            _cfg = cfg;
            _serviceProvider = serviceProvider;
            _devices = BuildDevices();
            _sampleDefinitions = _cfg.GetSection("samples").Get<SampleDefinition[]>();
        }


        public async Task TickAsync()
        {
            if (_sampleDefinitions != null)
            {
                Console.WriteLine("Tick");

                var due = _sampleDefinitions.Where(d => d.IsDue(DateTime.UtcNow));

                if (due.Any())
                {
                    Console.WriteLine($" - {due.Count()} due");
                    foreach (var sampleDefinition in due)
                    {
                        try
                        {
                            sampleDefinition.IsInProgress = true;
                            var device = _devices.SingleOrDefault(d => d.Name.EqualsNoCase(sampleDefinition.Device));
                            if (device == null)
                            {
                                throw new NullReferenceException($"Device for {sampleDefinition.Name} not found ({sampleDefinition.Device})");
                            }

                            var val = await device.ReadAsync(sampleDefinition.Cfg);
                            if (val.HasValue)
                            {
                                Console.Write($"{sampleDefinition.Name}: {sampleDefinition.LastValue:0.000} -> {val:0.000}");

                                if (sampleDefinition.CanUpdate(val.Value))
                                {
                                    sampleDefinition.LastValue = val;
                                    Console.Write(" - updated");
                                }
                                else
                                {
                                    Console.Write(" - no change");
                                }

                                Console.WriteLine();

                                sampleDefinition.LastSampleOn = DateTime.UtcNow;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"'{sampleDefinition.Name}' failed:\n" + ex);
                        }
                        finally
                        {
                            sampleDefinition.IsInProgress = false;
                        }
                    }
                }
            }
        }


        private IEnumerable<IDevice> BuildDevices()
        {
            // @todo: Should probably be a Dictionary<string, IDevice>
            var devices = new List<IDevice>();

            var deviceDefinitions = _cfg.GetSection("devices").Get<DeviceConfig[]>();
            foreach (var definition in deviceDefinitions)
            {
                Console.WriteLine($" - Creating device {definition.Name} ({definition.Type})");

                var device = (IDevice)_serviceProvider.GetService(Type.GetType($"Agent.Devices.{definition.Type}"));
                if (device != null)
                {
                    device.Initialise(definition);
                    devices.Add(device);
                }
            }

            return devices;
        }
    }
}
