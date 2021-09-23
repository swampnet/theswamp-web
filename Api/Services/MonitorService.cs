using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Api.DAL;
using TheSwamp.Api.Interfaces;
using TheSwamp.Shared;

namespace TheSwamp.Api.Services
{
    internal class MonitorService : IMonitor
    {
        private readonly TrackingContext _trackingContext;

        public MonitorService(TrackingContext trackingContext)
        {
            _trackingContext = trackingContext;
        }

        public async Task<Device[]> GetDevicesAsync()
        {
            var x = await _trackingContext.Devices.ToArrayAsync();

            return x.Select(y => new Device() { 
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                CreatedOnUtc = y.CreatedOnUtc
            }).ToArray();
        }


        public async Task<Device> GetDeviceAsync(string deviceName)
        {
            var x = await _trackingContext.Devices.Where(x => x.Name == deviceName).SingleOrDefaultAsync();

            if(x == null)
            {
                // create device
                x = new DAL.Entities.Device()
                {
                    Name = deviceName,
                    CreatedOnUtc = DateTime.UtcNow
                };
                _trackingContext.Devices.Add(x);

                await _trackingContext.SaveChangesAsync();
            }

            return new Device()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                CreatedOnUtc = x.CreatedOnUtc
            };
        }

        public async Task<DeviceValue[]> GetValuesAsync (int deviceId)
        {
            var x = await _trackingContext.DeviceValues
                .Where(dv => dv.DeviceId == deviceId)
                .ToArrayAsync();

            return x.Select(y => new DeviceValue()
            {
                TimestampUtc = y.TimestampUtc,
                Value = y.Value
            }).ToArray();
        }


        public async Task PostValuesAsync(DeviceValue[] deviceValues)
        {
            foreach(var grpd in deviceValues.GroupBy(d => d.DeviceId))
            {
                var device = await _trackingContext.Devices.SingleOrDefaultAsync(d => d.Id == grpd.Key);
                if(device != null)
                {
                    device.Values = grpd
                        .Select(d => 
                            new DAL.Entities.DeviceValue() { 
                                TimestampUtc = d.TimestampUtc,
                                Value = d.Value
                            })
                        .ToList();

                    await _trackingContext.SaveChangesAsync();
                }
            }
        }
    }
}
