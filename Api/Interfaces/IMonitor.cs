using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace TheSwamp.Api.Interfaces
{
    public interface IMonitor
    {
        public Task<DataSourceSummary[]> GetDataSourceSummaryAsync();
        public Task<DataSource[]> GetDevicesAsync();
        Task<DataPoint[]> GetValuesAsync(int deviceId);
        Task<DataSource> GetDeviceAsync(string deviceName);
        Task PostValuesAsync(DataPoint[] deviceValues);
        Task<DataSourceSummary> GetHistory(string device);
    }
}
