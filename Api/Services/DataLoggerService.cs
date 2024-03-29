﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<DataSourceDetails> GetHistory(string device)
        {
            var MAX = 100;

            var details = await _trackingContext.DataSources
                .Select(x => new DataSourceDetails()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Units = x.Units,
                    Name = x.Name,
                    LastUpdateOnUtc = x.Values.OrderByDescending(v => v.TimestampUtc).FirstOrDefault().TimestampUtc,
                    LastValue = x.Values.OrderByDescending(v => v.TimestampUtc).FirstOrDefault().Value,
                    UpdateCount = x.Values.Count(),
                    Values = x.Values.OrderByDescending(x => x.TimestampUtc).Take(MAX).ToArray(),
                    Processors = x.Processors.Where(p => p.IsActive).Select(p => new ProcessorSummary()
                    {
                        Name = p.Name,
                        Parameters = p.Parameters.Select(x => new Property()
                        {
                            Name = x.Name,
                            Value = x.Value
                        }).ToArray()
                    })
                    .ToArray()
                })
                .SingleOrDefaultAsync(x => x.Name == device);

            var from = details.Values.Min(x => x.TimestampUtc);
            var to = details.Values.Max(x => x.TimestampUtc);

            if(details != null)
            {
                details.Events = await _trackingContext.Events.Where(e => e.DataSourceId == details.Id && e.TimestampUtc >= from && e.TimestampUtc <= to).Select(e => new DataSourceEventSummary()
                {
                    DataSourceId = e.DataSourceId,
                    Description = e.Description,
                    TimestampUtc = e.TimestampUtc,
                    DataPoint = new DataPoint()
                    {
                        DataSourceId = e.DataSourceId,
                        TimestampUtc = e.DataPoint.TimestampUtc,
                        Value = e.DataPoint.Value
                    }
                }).ToArrayAsync();
            }

            return details;
        }


        public Task<DataSourceDetails[]> GetDataSourceSummaryAsync()
        {
            var query = _trackingContext.DataSources
                .Select(x => new DataSourceDetails()
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
                                        Description = rs.Summary,
                                        DataPoint = val
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
