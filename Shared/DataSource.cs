using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace TheSwamp.Shared
{
    public class DataSourceSummary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Units { get; set; }
        public DateTime? LastUpdateOnUtc { get; set; }
        public string LastValue { get; set; }
        public int UpdateCount { get; set; }
    }


    public class DataSource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public bool UseAverage { get; set; }
        public int? AveragePrecision { get; set; }
        public string Units { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {Name}";
        }
    }


    public class DataPoint
    {
        // well this is dumb. azure functions uses JSON.Net, Blazor uses System.Text.Json
        // i might just drop the attributes and rename the members directly
        [JsonPropertyName("s")]
        [JsonProperty("s")]
        public int DataSourceId { get; set; }

        [JsonPropertyName("t")]
        [JsonProperty("t")]
        public DateTime TimestampUtc { get; set; }

        [JsonPropertyName("v")]
        [JsonProperty("v")]
        public string Value { get; set; }
    }
}
