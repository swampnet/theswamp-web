using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TheSwamp.Api.DAL;
using TheSwamp.Api.Interfaces;
using TheSwamp.Api.Services;

[assembly: FunctionsStartup(typeof(TheSwamp.Api.Startup))]
namespace TheSwamp.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var context = builder.GetContext();
            var cfg = new ConfigurationBuilder()
                .SetBasePath(context.ApplicationRootPath)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(cfg);
            builder.Services.AddHttpClient();

            builder.Services.AddTransient<IMyService, MyService>();
            builder.Services.AddTransient<IMonitor, MonitorService>();
            builder.Services.AddTransient<IAuth, Auth>();

            builder.Services.AddDbContext<TrackingContext>(options =>
                options.UseSqlServer(cfg["connectionstring.swampnet"])
            );
        }
    }
}
