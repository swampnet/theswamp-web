using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent.Devices
{
    abstract class DeviceBase : IDevice
    {
        private bool _isConfigured = false;

        public string Name { get; private set; }
        public Property[] Cfg { get; private set; }

        public void Initialise(DeviceConfig cfg)
        {
            if (_isConfigured)
            {
                throw new InvalidOperationException($"{cfg.Name} / Type '{this.GetType().Name}' already configured");
            }

            Name = cfg.Name;
            Cfg = cfg.Cfg;
            
            Initialise(Cfg);
            
            _isConfigured = true;
        }

        internal virtual void Initialise(IEnumerable<IProperty> cfg)
        {
        }
        
        public abstract Task<double?> ReadAsync(IEnumerable<IProperty> parameters);
    }
}
