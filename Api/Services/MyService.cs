using System;
using System.Collections.Generic;
using System.Text;
using TheSwamp.Api.Interfaces;

namespace TheSwamp.Api.Services
{
    internal class MyService : IMyService
    {
        private int _count = 0;
        public string Boosh()
        {
            _count++;
            return $"bosh#{_count} @ {DateTime.Now}!";
        }
    }
}
