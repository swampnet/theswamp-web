using System;
using System.Collections.Generic;
using System.Text;

namespace TheSwamp.Api.DAL.Entities
{
    public class Device : Shared.Device
    {
        public ICollection<DeviceValue> Values { get; set; }
    }

    public class DeviceValue : Shared.DeviceValue
    {
        public long Id { get; set; }
        public Device Device { get; set; }
    }
}
