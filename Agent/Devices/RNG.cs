using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent.Devices
{
    class RNG : DeviceBase
    {
        private Random _rnd = new Random();
        

        public override Task<double?> ReadAsync(IEnumerable<IProperty> parameters)
        {
            double? val = null;
            var min = parameters.IntValue("min", 0);
            var max = parameters.IntValue("max", 100);
            if(parameters.BooleanValue("int-values-only", false))
            {
                val = (double)_rnd.Next(min, max);
            }
            else
            {
                val = min + (_rnd.NextDouble() * max);
            }

            return Task.FromResult(val);
        }
    }
}
