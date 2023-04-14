using System;
using System.Collections.Generic;
using System.Text;

namespace TheSwamp.Shared
{
    public enum ReviewPersonality
    {
        Pretentious,
        Gangster,
        BadFrench,
        Rhyme,
        Comedian,
        Rap,
        Drunk,
        None
    }

    public class Wine
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Vintage { get; set; }
        public string Colour { get; set; }
        public string ProducerName { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }

        public string Type { get; set; }
        public string SubType { get; set; }
        public string SubRegion { get; set; }
    }
}
