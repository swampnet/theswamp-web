using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Shared;
using TheSwamp.Api.Interfaces;

namespace TheSwamp.Api.Services.DataPointProcessors
{
    internal class SquirtyBoi : IDataPointProcessor
    {
        private readonly IPostMessage _postMessage;

        public string Name => "activate-pump";

        public SquirtyBoi(IPostMessage postMessage)
        {
            _postMessage = postMessage;
        }


        public async Task<ProcessDataSourceResult> ProcessAsync(DAL.TRK.Entities.DataSourceProcessor source, DAL.TRK.Entities.DataPoint pt)
        {
            var rs = new ProcessDataSourceResult();

            var val = Convert.ToDouble(pt.Value);
            var threshold = source.Parameters.DoubleValue("threshold");
            var channel = source.Parameters.StringValue("channel");
            
            if (val > threshold)
            {
                await _postMessage.PostAsync(new AgentMessage()
                {
                    Type = "activate-pump",
                    Properties = new List<Property>()
                    {
                        new Property("channel", channel)
                    }
                });

                rs.Summary = $"{source.DataSource.Name} ({source.DataSource.Description}) - Activating pump {channel}";
                rs.Broadcast = true;
            }

            return rs;
        }
    }
}
