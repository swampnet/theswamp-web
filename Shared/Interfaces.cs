using System;
using System.Collections.Generic;
using System.Text;

namespace TheSwamp.Shared
{
    public interface IProperty
    {
        string Name { get; }
        string Value { get; }
    }
}
