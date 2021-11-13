using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSwamp.Api.Interfaces;
using TheSwamp.Shared;
using DataPoint = TheSwamp.Api.DAL.TRK.Entities.DataPoint;
using DataSource = TheSwamp.Api.DAL.TRK.Entities.DataSource;
using DataSourceEvent = TheSwamp.Api.DAL.TRK.Entities.DataSourceEvent;

namespace TheSwamp.Api.Services.DataPointProcessors
{
    internal class RandomNumberProcessor : IDataPointProcessor
    {
        public RandomNumberProcessor()
        {
        }


        public Task ProcessAsync(DataSource source, DataPoint pt)
        {
            if (source.Name.EqualsNoCase("test-01") && Convert.ToInt32(pt.Value) > 13)
            {
                source.Events.Add(new DataSourceEvent() { 
                    DataSourceId = pt.DataSourceId,
                    TimestampUtc = pt.TimestampUtc,
                    Description = $"Rolled {pt.Value}"
                });
            }

            return Task.CompletedTask;
        }
    }
}
