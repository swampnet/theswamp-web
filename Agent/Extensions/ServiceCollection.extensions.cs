using Agent.Devices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent
{
    static class ServiceCollectionExtensions
    {
        public static ServiceCollection UseSampleCollection(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISamples, SamplesService>();

            serviceCollection.AddTransient<RNG>();
            serviceCollection.AddTransient<A2D>();
            serviceCollection.AddSingleton<Hub>();

            return serviceCollection;
        }


        public static ServiceCollection UseQueueHandlers(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<AgentQueueHandler>();

            serviceCollection.AddSingleton<IQueueHandler, QueueHandlers.LedMatrixMessageHandler>();
            serviceCollection.AddSingleton<IQueueHandler, QueueHandlers.ActivatePumpMessageHandler>();

            return serviceCollection;
        }
    }
}
