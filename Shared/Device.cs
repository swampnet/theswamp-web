using Newtonsoft.Json;
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
        [JsonProperty("d")]
        public int DeviceId { get; set; }
        
        [JsonProperty("t")]
        public DateTime TimestampUtc { get; set; }
        
        [JsonProperty("v")]
        public string Value { get; set; }
    }
}
