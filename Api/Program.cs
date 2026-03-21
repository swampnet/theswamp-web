using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.GPT3.Extensions;
using TheSwamp.Api.DAL.API;
using TheSwamp.Api.DAL.IOT;
using TheSwamp.Api.DAL.LWIN;
using TheSwamp.Api.DAL.TRK;
using TheSwamp.Api.Interfaces;
using TheSwamp.Api.Services;
using TheSwamp.Api.Services.DataPointProcessors;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        var cfg = new ConfigurationBuilder()
            .SetBasePath(context.HostingEnvironment.ContentRootPath)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        services.AddSingleton<IConfiguration>(cfg);
        services.AddSingleton<ICache, Cache>();

        services.AddHttpClient();

        services.AddTransient<IDataLogger, DataLoggerService>();
        services.AddTransient<IAuth, Auth>();
        services.AddTransient<IPostMessage, PostMessageService>();

        services.AddTransient<IDataPointProcessor, RaiseEventOnValue>();
        services.AddTransient<IDataPointProcessor, SquirtyBoi>();

        services.AddOpenAIService(settings => { settings.ApiKey = cfg["openai.apikey"]; });

        services.AddDbContext<TrackingContext>(options =>
            options.UseSqlServer(cfg["connectionstring.swampnet"]));

        services.AddDbContext<ApiContext>(options =>
            options.UseSqlServer(cfg["connectionstring.swampnet"]));

        services.AddDbContext<IotContext>(options =>
            options.UseSqlServer(cfg["connectionstring.swampnet"]));

        services.AddDbContext<LWINContext>(options =>
            options.UseSqlServer(cfg["connectionstring.swampnet"]));
    })
    .Build();

host.Run();
