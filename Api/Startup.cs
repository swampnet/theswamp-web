using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TheSwamp.Api.Interfaces;
using TheSwamp.Api.Services;

[assembly: FunctionsStartup(typeof(TheSwamp.Api.Startup))]
namespace TheSwamp.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            //builder.AddUserSecrets<Startup>();

            builder.Services.AddSingleton<IMyService, MyService>();
        }
    }
}
