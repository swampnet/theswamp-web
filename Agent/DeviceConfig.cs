using Microsoft.Extensions.Configuration;
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
    public class DeviceConfig
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Property[] Cfg { get; set; }
    }
}
