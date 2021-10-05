using System;
using System.Collections.Generic;
using System.Text;

namespace TheSwamp.Shared
{
    public class AgentMessage
    {
        public AgentMessage()
        {
            CreatedOnUtc = DateTime.UtcNow;
        }

        public DateTime CreatedOnUtc { get; set; }
        public string Message { get; set; }
    }
}
