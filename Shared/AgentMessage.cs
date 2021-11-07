using System;
using System.Collections.Generic;
using System.Text;

namespace TheSwamp.Shared
{
    public class AgentMessage
    {
        public AgentMessage()
        {
            TimestampUtc = DateTime.UtcNow;
            Properties = new List<Property>();
        }

        public AgentMessage(string type)
            : this()
        {
            Type = type;
        }

        public DateTime TimestampUtc { get; set; }
        public string Type { get; set; }
        public List<Property> Properties { get; set; }
    }
}
