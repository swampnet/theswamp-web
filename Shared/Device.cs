using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace TheSwamp.Shared
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOnUtc { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {Name}";
        }
    }


    public class DeviceValue
    {
        // well this is dumb. azure functions uses JSON.Net, Blazor uses System.Text.Json
        // i might just drop the attributes and rename the members directly
        [JsonPropertyName("d")]
        [JsonProperty("d")]
        public int DeviceId { get; set; }

        [JsonPropertyName("t")]
        [JsonProperty("t")]
        public DateTime TimestampUtc { get; set; }

        [JsonPropertyName("v")]
        [JsonProperty("v")]
        public string Value { get; set; }
    }
}
