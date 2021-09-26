using Microsoft.Extensions.Configuration;
using System;
using TheSwamp.Shared;

namespace Agent
{
    internal class Program
    {
        private static IConfiguration _cfg;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            API.Initialise(_cfg["api:endpoint"], _cfg["api:key"]);


        }
    }
}
