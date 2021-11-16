using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace TheSwamp.Api.Interfaces
{
    public interface IDataLogger
    {
        public Task<DataSourceDetails[]> GetDataSourceSummaryAsync();
        public Task<DataSource[]> GetDevicesAsync();
        //Task<DataPoint[]> GetValuesAsync(int deviceId);
        Task<DataSource> GetDeviceAsync(string deviceName);
        Task PostValuesAsync(DataPoint[] deviceValues);
        Task<DataSourceDetails> GetHistory(string device);
    }


    public interface IDataPointProcessor
    {
        string Name { get; }
        Task<ProcessDataSourceResult> ProcessAsync(DAL.TRK.Entities.DataSourceProcessor source, DAL.TRK.Entities.DataPoint pt);
    }
}
