﻿using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSwamp.Shared;
using Microsoft.Extensions.DependencyInjection;
using Agent.Devices;
using System.Threading;

namespace Agent
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder();
#if DEBUG
                builder.AddJsonFile("local.settings.json", optional: false, reloadOnChange: true);
#else
                builder.AddJsonFile("settings.json", optional: false, reloadOnChange: true);
#endif
                var cfg = builder.Build();

                API.Initialise(cfg["api:endpoint"], cfg["api:key"]);

                var serviceCollection = new ServiceCollection();
                serviceCollection.AddSingleton<Program>();
                serviceCollection.AddSingleton<IConfiguration>(cfg);
                
                serviceCollection.UseSampleCollection();
                serviceCollection.UseQueueHandlers();
                                
                await serviceCollection.BuildServiceProvider()
                    .GetService<Program>()
                    .RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("bye!");
        }

        private int _tick = 0;
        private readonly ISamples _samples;
        private readonly IConfiguration _cfg;
        private readonly AgentQueueHandler _queueHandler;

        public Program(IConfiguration cfg, ISamples samples, AgentQueueHandler queueHandler)
        {
            _samples = samples;
            _cfg = cfg;
            _queueHandler = queueHandler;
        }


        public async Task RunAsync()
        {
            await _queueHandler.StartAsync();

            // Main loop
            while (true)
            {
                try
                {
                    await _samples.TickAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    await Wait();
                }
            }
        }


        private async Task Wait()
        {
            var x = Convert.ToInt32(_cfg["tick"]);
            if (x <= 0)
            {
                x = 1000 * 60; // Default to one minute
            }

            if (x != _tick)
            {
                _tick = x;
                Console.WriteLine($"Tick delay: {_tick}");
            }

            await Task.Delay(_tick);
        }

        #region Legacy
        //private static async Task<bool> ProcessMessages(string[] args)
        //{
        //    var msg = args.Where(a => a.StartsWith("led-message:"));
        //    if (msg.Any())
        //    {
        //        foreach (var m in msg)
        //        {
        //            var content = m.Substring(m.IndexOf(":") + 1).Replace("_", " ");
        //            Console.WriteLine($"msg: {content}");
        //            await API.PostMessageAsync(new AgentMessage()
        //            {
        //                Type = "led-matrix",
        //                Properties = new List<Property>() {
        //                    new Property("content", content)
        //                }
        //            });
        //        }

        //        return true;
        //    }
        //    return false;
        //}


        //private static async Task MainLoopAsync()
        //{
        //    Console.WriteLine("Running main loop");

        //    var lastRun = new Dictionary<ISampleProvider, DateTime>();
        //    var monitor = new Monitor(60000 * 5);
        //    //var monitor = new Monitor(10000);

        //    // @TODO: Inject this stuff
        //    var things = new List<ISampleProvider>()
        //    {
        //        new RandomNumberThing(),
        //        //new SendMessageThing(),
        //        new SensorHubThing(),
        //        new Mcp3008Things(),
        //        new CpuTemperatureThing()
        //    };

        //    var queueHandler = new AgentQueueHandler(Cfg);
        //    await queueHandler.StartAsync();

        //    while (true)
        //    {
        //        foreach (var t in things)
        //        {
        //            try
        //            {
        //                if (!lastRun.ContainsKey(t))
        //                {
        //                    Console.WriteLine($"Add {t.GetType().Name}");
        //                    lastRun.Add(t, DateTime.MinValue);
        //                }

        //                var diff = (DateTime.UtcNow - lastRun[t]).TotalSeconds;
        //                if (diff > t.PollInterval.TotalSeconds)
        //                {
        //                    //Console.WriteLine($"[{DateTime.UtcNow}] Poll {t.GetType().Name} ({diff})");
        //                    await t.PollAsync(monitor);
        //                    lastRun[t] = DateTime.UtcNow;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(ex);
        //            }
        //        }

        //        await Task.Delay(500);
        //    }
        //}

        #endregion
    }
}
