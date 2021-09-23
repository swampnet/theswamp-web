using System;

namespace TheSwamp.Shared
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }

    public class DeviceValue
    {
        public DateTime TimestampUtc { get; set; }
        public string Value { get; set; }
    }
}
