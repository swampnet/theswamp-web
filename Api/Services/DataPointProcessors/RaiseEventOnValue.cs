using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSwamp.Api.Interfaces;
using TheSwamp.Shared;

using DataPoint = TheSwamp.Api.DAL.TRK.Entities.DataPoint;
using DataSourceEvent = TheSwamp.Api.DAL.TRK.Entities.DataSourceEvent;
using DataSourceProcessor = TheSwamp.Api.DAL.TRK.Entities.DataSourceProcessor;

namespace TheSwamp.Api.Services.DataPointProcessors
{
    internal class RaiseEventOnValue : IDataPointProcessor
    {
        public string Name => "check-value";

        public Task<ProcessDataSourceResult> ProcessAsync(DataSourceProcessor source, DataPoint pt)
        {
            var rs = new ProcessDataSourceResult();
            var val = source.Parameters.IntValue("gt", int.MinValue);
            if(val != int.MinValue && Convert.ToInt32(pt.Value) > val)
            {
                rs.Summary = $"Rolled a {pt.Value}";
                rs.Broadcast = true;
            }

            return Task.FromResult(rs);
        }
    }
}
