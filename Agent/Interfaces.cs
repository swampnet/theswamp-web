using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent
{
    interface ISamples
    {
        Task TickAsync();
    }

    interface IDevice
    {
        string Name { get; }
        void Initialise(DeviceConfig cfg);
        Task<double?> ReadAsync(IEnumerable<IProperty> parameters);
    }

    interface IQueueHandler
    {
        bool CanProcess(AgentMessage msg);
        Task ProcessAsync(AgentMessage msg);
    }
}
