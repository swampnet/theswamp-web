using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheSwamp.Shared
{
    public class Monitor
    {
        private readonly Timer _timer;
        private readonly Dictionary<string, DataSourcePointsCache> _dataSource = new Dictionary<string, DataSourcePointsCache>();

        public Monitor(int period = 60 * 1000)
        {
            _timer = new Timer(Flush, null, 10000, period);
        }


        public async Task AddDataPointAsync(string dataSourceName, object value)
        {
            if (!_dataSource.ContainsKey(dataSourceName))
            {
                var ds = await API.GetDeviceAsync(dataSourceName);
                lock (_dataSource)
                {
                    if (!_dataSource.ContainsKey(dataSourceName))
                    {
                        _dataSource.Add(dataSourceName, new DataSourcePointsCache(ds));
                    }
                }
            }

            lock (_dataSource)
            {
                var ds = _dataSource[dataSourceName];
                var newValue = value?.ToString();

                if (ds.LatestValue != newValue)
                {
                    ds.Values.Add(new DataPoint()
                    {
                        DataSourceId = ds.DataSource.Id,
                        TimestampUtc = DateTime.UtcNow,
                        Value = newValue
                    });

                    ds.LatestValue = newValue;
                }
            }
        }


        public async Task FlushAsync()
        {
            var data = new List<DataPoint>();

            lock (_dataSource)
            {
                foreach (var xx in _dataSource.Values)
                {
                    data.AddRange(xx.Values);
                    xx.Values.Clear();
                }

            }

            await API.PostDataAsync(data);

            Debug.WriteLine($"flushed {data.Count()} items");
        }


        private async void Flush(object state)
        {
            try
            {
                await FlushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }


        class DataSourcePointsCache
        {
            public DataSourcePointsCache(DataSource ds)
            {
                DataSource = ds;
                Values = new List<DataPoint>();
                LatestValue = null;
            }

            public string LatestValue { get; set; }
            public DataSource DataSource { get; private set; }
            public List<DataPoint> Values { get; private set; }
        }
    }
}
