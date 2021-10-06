using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Api.DAL;
using TheSwamp.Api.DAL.TRK;
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

        public Task<DataSourceSummary[]> GetDataSourceSummaryAsync()
        {
            var query = _trackingContext.DataSources
                .Select(x => new DataSourceSummary()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Units = x.Units,
                    Name = x.Name,
                    LastUpdateOnUtc = x.Values.OrderByDescending(v => v.TimestampUtc).FirstOrDefault().TimestampUtc,
                    LastValue = x.Values.OrderByDescending(v => v.TimestampUtc).FirstOrDefault().Value,                    
                    UpdateCount = x.Values.Count()
                });

            return query.ToArrayAsync();
        }


        public async Task<DataSource[]> GetDevicesAsync()
        {
            var x = await _trackingContext.DataSources.ToArrayAsync();

            return x.Select(y => new DataSource() { 
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                CreatedOnUtc = y.CreatedOnUtc
            }).ToArray();
        }


        public async Task<DataSource> GetDeviceAsync(string deviceName)
        {
            var x = await _trackingContext.DataSources.Where(x => x.Name == deviceName).SingleOrDefaultAsync();

            if(x == null)
            {
                // create device
                x = new DAL.TRK.Entities.DataSource()
                {
                    Name = deviceName,
                    CreatedOnUtc = DateTime.UtcNow,
                    UseAverage = false 
                };
                _trackingContext.DataSources.Add(x);

                await _trackingContext.SaveChangesAsync();
            }

            return new DataSource()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                UseAverage = x.UseAverage,
                AveragePrecision = x.AveragePrecision,
                CreatedOnUtc = x.CreatedOnUtc
            };
        }

        public async Task<DataPoint[]> GetValuesAsync (int deviceId)
        {
            var x = await _trackingContext.DataPoints
                .Where(dv => dv.DataSourceId == deviceId)
                .ToArrayAsync();

            return x.Select(y => new DataPoint()
            {
                TimestampUtc = y.TimestampUtc,
                Value = y.Value
            }).ToArray();
        }


        public async Task PostValuesAsync(DataPoint[] deviceValues)
        {
            foreach(var grpd in deviceValues.GroupBy(d => d.DataSourceId))
            {
                var device = await _trackingContext.DataSources.SingleOrDefaultAsync(d => d.Id == grpd.Key);
                if(device != null)
                {
                    device.Values = grpd
                        .Select(d => 
                            new DAL.TRK.Entities.DataPoint() { 
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
