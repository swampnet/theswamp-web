using System;
using System.Collections.Generic;
using System.Text;

namespace TheSwamp.Shared
{
    public class Review
    {
        public Review()
        {
            Benchmarks = new List<Benchmark>();
            Notes = new List<string>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Blurb { get; set; }
        public string Tone { get; set; }


        public string Error { get; set; }
        public string Vintage { get; set; }
        public string SubType { get; set; }
        public string Colour { get; set; }

        public List<Benchmark> Benchmarks { get; set; }
        public List<string> Notes { get; set; }
        public string ProducerName { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Model { get; set; }
    }


    public class Benchmark
    {
        public Benchmark()
        {
        }

        public Benchmark(string name, TimeSpan elapsed)
            : this()
        {
            Name = name;
            Elapsed = elapsed;
        }

        public TimeSpan Elapsed { get; set; }
        public string Name { get; set; }
    }
}
