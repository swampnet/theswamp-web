using System;
using System.Collections.Generic;
using System.Text;

namespace TheSwamp.Api.DAL.TRK.Entities
{
    public class DataSource : Shared.DataSource
    {
        public DataSource()
        {
            Values = new List<DataPoint>();
            Events = new List<DataSourceEvent>();
        }

        public ICollection<DataPoint> Values { get; set; }
        public ICollection<DataSourceEvent> Events { get; set; }
        public ICollection<DataSourceProcessor> Processors { get; set; }
    }

    public class DataPoint : Shared.DataPoint
    {
        public long Id { get; set; }
        public DataSource Source { get; set; }

        public ICollection<DataSourceEvent> Events { get; set; }
    }


    public class DataSourceEvent
    {

        public long Id { get; set; }

        public int DataSourceId { get; set; }
        public DataSource Source { get; set; }

        public DateTime TimestampUtc { get; set; }

        public string Description { get; set; }

        public long? DataPointId { get; set; }
        public DataPoint DataPoint { get; set; }
    }


    public class DataSourceProcessor
    {
        public int Id { get; set; }
        public int DataSourceId { get; set; }
        public DataSource DataSource { get; set; }

        public string Name { get; set; }
        public bool IsActive { get; set; }

        public ICollection<DataSourceProcessorParameter> Parameters { get; set; }
    }


    public class DataSourceProcessorParameter : Shared.IProperty
    {
        public int Id { get; set; }
        public int DataSourceProcessorId { get; set; }
        public DataSourceProcessor Processor { get; set; }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
