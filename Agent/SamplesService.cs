using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent
{
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
                    
                    var values = new List<DataPoint>();

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

                            // Hook up external dataSource id we don't already have it
                            if(sampleDefinition.DataSource == null)
                            {
                                Console.Write($"Resolving datasource for '{sampleDefinition.Name}'");
                                sampleDefinition.DataSource = await API.GetDeviceAsync(sampleDefinition.Name);
                                Console.WriteLine($" - id: {sampleDefinition.DataSource.Id}");
                            }

                            var val = (await device.ReadAsync(sampleDefinition.Cfg)).Process(sampleDefinition);
                            if (val.HasValue)
                            {
                                sampleDefinition.LastSampleOn = DateTime.UtcNow;

                                Console.Write($"{sampleDefinition.Name}: {sampleDefinition.LastValue} -> {val}");

                                if (sampleDefinition.CanUpdate(val.Value))
                                {
                                    Console.Write(" - updated");
                                    sampleDefinition.LastValue = val;
                                    values.Add(new DataPoint() { 
                                        DataSourceId = sampleDefinition.DataSource.Id,
                                        Value = val.ToString(),
                                        TimestampUtc = sampleDefinition.LastSampleOn.Value
                                    });
                                }
                                else
                                {
                                    Console.Write(" - no change");
                                }

                                Console.WriteLine();

                                
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

                    if (values.Any())
                    {
                        Console.WriteLine($"Posting {values.Count()} values");
                        await API.PostDataAsync(values);
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
