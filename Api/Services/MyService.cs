using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheSwamp.Api.DAL;
using TheSwamp.Api.Interfaces;

namespace TheSwamp.Api.Services
{
    internal class MyService : IMyService
    {
        private readonly TrackingContext _deviceContext;
        private int _count = 0;

        public MyService(TrackingContext deviceContext)
        {
            _deviceContext = deviceContext;
        }

        public string Boosh()
        {
            _count++;

            var x = _deviceContext.DataSources.ToArray();

            return $"bosh#{_count} @ {DateTime.Now}!";
        }
    }
}
