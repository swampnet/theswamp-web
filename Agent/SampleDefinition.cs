using System;
using TheSwamp.Shared;

namespace Agent
{
    class SampleDefinition
    {
        public string Name { get; set; }
        public string Device { get; set; }
        public TimeSpan Frequency { get; set; }
        public double? MinChange { get; set; }
        public int? Precision { get; set; }
        public Property[] Cfg { get; set; }
        public DateTime? LastSampleOn { get; set; }
        public bool IsInProgress { get; internal set; } = false;
        public double? LastValue { get; internal set; }

        public DataSource DataSource { get; internal set; }
    }
}
