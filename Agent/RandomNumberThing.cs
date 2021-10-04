using System;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent
{
    public class RandomNumberThing : IThing
    {
        private readonly Random _rng;
        public TimeSpan PollInterval => TimeSpan.FromSeconds(60);

        public RandomNumberThing()
        {
            _rng = new Random();
        }

        public async Task PollAsync(Monitor monitor)
        {
            await monitor.AddDataPointAsync("test-01", _rng.Next(1,10));
            await monitor.AddDataPointAsync("test-02", _rng.NextDouble());

            await Task.CompletedTask;
        }
    }
}
