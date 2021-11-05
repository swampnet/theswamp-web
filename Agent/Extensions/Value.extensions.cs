using System;
using System.Collections.Generic;
using System.Text;

namespace Agent
{
    static class ValueExtensions
    {
        public static double? Process(this double? val, SampleDefinition sampleDefinition)
        {
            if (val.HasValue)
            {
                // Round if we specify a precision
                if (sampleDefinition.Precision.HasValue)
                {
                    val = Math.Round(val.Value, sampleDefinition.Precision.Value);
                }

                // @TODO: Potentially just store the value and set to null if we're averaging.
            }

            return val;
        }
    }
}
