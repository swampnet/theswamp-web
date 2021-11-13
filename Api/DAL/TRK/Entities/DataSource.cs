using System;
using System.Collections.Generic;
using System.Text;

namespace TheSwamp.Api.DAL.TRK.Entities
{
    public class DataSource : Shared.DataSource
    {
        public ICollection<DataPoint> Values { get; set; }
        public ICollection<DataSourceEvent> Events { get; set; }
    }

    public class DataPoint : Shared.DataPoint
    {
        public long Id { get; set; }
        public DataSource Source { get; set; }
    }

    public class DataSourceEvent : Shared.DataSourceEvent
    {
        public long Id { get; set; }
        public DataSource Source { get; set; }
    }
}
