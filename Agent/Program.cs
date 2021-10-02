using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var lastRun = new Dictionary<IThing, DateTime>();
            var cfg = new ConfigurationBuilder()
               .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
               .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
               //.AddCommandLine(args)
               .Build();

            API.Initialise(cfg["api:endpoint"], cfg["api:key"]);
            var monitor = new Monitor(60000 * 5);
            var things = new List<IThing>();

            //things.Add(new RandomNumberThing());
            things.Add(new SensorHubThing());


            while (true)
            {
                foreach(var t in things)
                {
                    try
                    {
                        if (!lastRun.ContainsKey(t))
                        {
                            Console.WriteLine($"Add {t.GetType().Name}");
                            lastRun.Add(t, DateTime.MinValue);
                        }

                        var diff = (DateTime.UtcNow - lastRun[t]).TotalSeconds;
                        if (diff > t.PollInterval.TotalSeconds)
                        {
                            Console.WriteLine($"[{DateTime.UtcNow}] Poll {t.GetType().Name} ({diff})");
                            await t.PollAsync(monitor);
                            lastRun[t] = DateTime.UtcNow;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                await Task.Delay(1000);
            }
        }
    }
}
