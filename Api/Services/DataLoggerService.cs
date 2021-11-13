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
    internal class DataLoggerService : IDataLogger
    {
        private readonly TrackingContext _trackingContext;
        private readonly IPostMessage _postMessage;
        private readonly IEnumerable<IDataPointProcessor> _dataPointProcessors;

        public DataLoggerService(TrackingContext trackingContext, IPostMessage postMessage, IEnumerable<IDataPointProcessor> dataPointProcessors)
        {
            _trackingContext = trackingContext;
            _postMessage = postMessage;
            _dataPointProcessors = dataPointProcessors;
        }

        public Task<DataSourceSummary> GetHistory(string device)
        {
            return _trackingContext.DataSources
                .Select(x => new DataSourceSummary()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Units = x.Units,
                    Name = x.Name,
                    LastUpdateOnUtc = x.Values.OrderByDescending(v => v.TimestampUtc).FirstOrDefault().TimestampUtc,
                    LastValue = x.Values.OrderByDescending(v => v.TimestampUtc).FirstOrDefault().Value,
                    UpdateCount = x.Values.Count(),
                    Values = x.Values.ToArray(),
                    Events = x.Events.ToArray()
                })
                .SingleOrDefaultAsync(x => x.Name == device);
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
                    CreatedOnUtc = DateTime.UtcNow
                };
                _trackingContext.DataSources.Add(x);

                await _trackingContext.SaveChangesAsync();
            }

            return new DataSource()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
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
                var dataSource = await _trackingContext.DataSources
                    .Include(f=>f.Processors)
                        .ThenInclude(f=>f.Parameters)
                    .SingleOrDefaultAsync(d => d.Id == grpd.Key);

                if(dataSource != null)
                {
                    var values = grpd
                        .Select(d =>
                            new DAL.TRK.Entities.DataPoint()
                            {
                                TimestampUtc = d.TimestampUtc,
                                Value = d.Value
                            })
                        .ToList();

                    dataSource.Values = values;

                    try
                    {
                        foreach(var p in dataSource.Processors.Where(p => p.IsActive))
                        {
                            var processor = _dataPointProcessors.SingleOrDefault(x => x.Name.EqualsNoCase(p.Name));
                            if(processor == null)
                            {
                                throw new NullReferenceException($"Processor '{p.Name}' not registered");
                            }

                            foreach(var val in values)
                            {
                                var rs = await processor.ProcessAsync(p, val);
                                if (!string.IsNullOrEmpty(rs.Summary))
                                {
                                    dataSource.Events.Add(new DAL.TRK.Entities.DataSourceEvent()
                                    {
                                        TimestampUtc = val.TimestampUtc,
                                        Description = rs.Summary
                                    });

                                    if (rs.Broadcast)
                                    {
                                        await _postMessage.PostAsync(new AgentMessage()
                                        {
                                            Type = "led-matrix",
                                            Properties = new List<Property>()
                                            {
                                                new Property()
                                                {
                                                    Name = "content",
                                                    Value = rs.Summary
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //log & ignore
                    }

                    await _trackingContext.SaveChangesAsync();
                }
            }
        }
    }


}
