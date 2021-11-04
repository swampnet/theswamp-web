using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent
{
    static class SampleDefinitionExtensions
    {
        public static bool IsDue(this SampleDefinition source, DateTime dt)
        {
            return !source.IsInProgress
                    && (!source.LastSampleOn.HasValue || source.LastSampleOn.Value.Add(source.Frequency) < dt);
        }

        /// <summary>
        /// Return if value has changed *enough* for us to care about!
        /// </summary>
        /// <param name="sampleDefinition"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool CanUpdate(this SampleDefinition sampleDefinition, double val)
        {
            var l = sampleDefinition.MinChange.HasValue ? sampleDefinition.MinChange.Value : 0.0;
            var diff = sampleDefinition.LastValue.HasValue
                ? Math.Abs(sampleDefinition.LastValue.Value - val)
                : (double?)null;

            return !diff.HasValue || diff > l;
        }
    }
}
