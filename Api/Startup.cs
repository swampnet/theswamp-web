using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.GPT3.Extensions;
using System;
using TheSwamp.Api.DAL.API;
using TheSwamp.Api.DAL.IOT;
using TheSwamp.Api.DAL.LWIN;
using TheSwamp.Api.DAL.TRK;
using TheSwamp.Api.Interfaces;
using TheSwamp.Api.Services;
using TheSwamp.Api.Services.DataPointProcessors;

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
            builder.Services.AddSingleton<ICache, Cache>();

            builder.Services.AddHttpClient();

            builder.Services.AddTransient<IDataLogger, DataLoggerService>();
            builder.Services.AddTransient<IAuth, Auth>();
            builder.Services.AddTransient<IPostMessage, PostMessageService>();

            builder.Services.AddTransient<IDataPointProcessor, RaiseEventOnValue>();
            builder.Services.AddTransient<IDataPointProcessor, SquirtyBoi>();

            builder.Services.AddOpenAIService(settings => { settings.ApiKey = cfg["openai.apikey"]; });

            builder.Services.AddDbContext<TrackingContext>(options =>
                options.UseSqlServer(cfg["connectionstring.swampnet"])
            );

            builder.Services.AddDbContext<ApiContext>(options =>
                options.UseSqlServer(cfg["connectionstring.swampnet"])
            );

            builder.Services.AddDbContext<IotContext>(options =>
                options.UseSqlServer(cfg["connectionstring.swampnet"])
            );

            builder.Services.AddDbContext<LWINContext>(options =>
                options.UseSqlServer(cfg["connectionstring.swampnet"])
            );
        }
    }
}
