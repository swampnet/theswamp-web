using System;
using System.Collections.Generic;
using System.Text;

namespace TheSwamp.Shared
{


    public class Property : IProperty
    {
        public Property()
        {
        }

        public Property(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
