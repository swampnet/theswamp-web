using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TheSwamp.Shared
{
    public class Monitor
    {
        private readonly Timer _timer;
        private readonly Dictionary<string, DataSourcePointsCache> _dataSource = new Dictionary<string, DataSourcePointsCache>();

        public Monitor(int flushPeriod = 60 * 1000)
        {
            _timer = new Timer(Flush, null, 10000, flushPeriod);
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
                        Console.WriteLine($"Adding datasource {dataSourceName}");
                        _dataSource.Add(dataSourceName, new DataSourcePointsCache(ds));
                    }
                }
            }

            lock (_dataSource)
            {
                var ds = _dataSource[dataSourceName];
                var newValue = value?.ToString();

                // If we're averaging, log everything - we take an average when we post it
                if (ds.DataSource.UseAverage)
                {
                    ds.Values.Add(new DataPoint()
                    {
                        DataSourceId = ds.DataSource.Id,
                        TimestampUtc = DateTime.UtcNow,
                        Value = newValue
                    });
                }

                // Not averaging, just log changes
                else
                {
                    if (CanLog(ds, newValue))
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
        }

        private bool CanLog(DataSourcePointsCache ds, string value)
        {
            if (double.TryParse(ds.LatestValue, out double l) && double.TryParse(value, out double n))
            {
                var diff = Math.Abs(l - n);
                if (ds.DataSource.AveragePrecision.HasValue)
                {
                    return diff > ds.DataSource.AveragePrecision.Value;
                }
                return diff > 0;
            }
            return true;
        }


        public async Task FlushAsync()
        {
            var data = new List<DataPoint>();

            foreach (var xx in _dataSource.Keys)
            {
                lock (xx)
                {
                    var c = _dataSource[xx];
                    if (c.Values.Any())
                    {
                        if (c.DataSource.UseAverage)
                        {
                            var avg = c.Values.Where(v => double.TryParse(v.Value, out double d)).Average(v => double.Parse(v.Value));
                            if (c.DataSource.AveragePrecision.HasValue)
                            {
                                avg = Math.Round(avg, c.DataSource.AveragePrecision.Value);
                            }

                            var savg = avg.ToString();

                            Console.WriteLine($"[{c.DataSource.Name}] Averaging {c.Values.Count()} values ({savg})");

                            if (savg != c.LastValue)
                            {
                                data.Add(new DataPoint()
                                {
                                    DataSourceId = c.DataSource.Id,
                                    TimestampUtc = DateTime.UtcNow,
                                    Value = savg
                                });

                                c.LastValue = savg;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[{c.DataSource.Name}] Posting {c.Values.Count()} values");
                            data.AddRange(c.Values);
                        }
                        c.Values.Clear();
                    }
                }
            }

            await API.PostDataAsync(data);

            Console.WriteLine($"[{DateTime.Now:HH\\:mm}] Flushed {data.Count()} values");
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

            public DataSource DataSource { get; private set; }
            public List<DataPoint> Values { get; private set; }

            // LAst average value we posted
            public string LastValue { get; internal set; }

            // LAst value logged
            public string LatestValue { get; set; }
        }
    }
}
