using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Api.DAL.LWIN.Entities;
using TheSwamp.Shared;

namespace TheSwamp.Api
{
    internal static class ConvertExtensions
    {
        public static Wine Convert(this LWINRaw w)
        {
            return new Wine()
            {
                Id = w.LWIN,
                Colour = w.COLOUR,
                Country = w.COUNTRY,
                Name = w.DISPLAY_NAME,
                ProducerName = w.PRODUCER_NAME,
                Region = w.REGION,
                SubRegion = w.SUB_REGION,
                Type = w.TYPE,
                SubType = w.SUB_TYPE,
                Vintage = w.FINAL_VINTAGE
            };
        }
    }
}
