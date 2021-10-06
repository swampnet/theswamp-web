using System;
using System.Collections.Generic;
using System.Text;

namespace TheSwamp.Shared
{
    public class AgentMessage
    {
        public AgentMessage()
        {
            Properties = new List<Property>();
        }

        public AgentMessage(string type)
            : this()
        {
            Type = type;
        }


        public string Type { get; set; }
        public List<Property> Properties { get; set; }
    }


    public class Property : IProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
