using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace TheSwamp.Api.Interfaces
{
    public interface IMonitor
    {
        public Task<Device[]> GetDevicesAsync();
        Task<DeviceValue[]> GetValuesAsync(int deviceId);
        Task<Device> GetDeviceAsync(string deviceName);
        Task PostValuesAsync(DeviceValue[] deviceValues);
    }
}
