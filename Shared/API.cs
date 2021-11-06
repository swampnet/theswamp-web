using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TheSwamp.Shared
{
    public static class API
    {
        private static HttpClient _client = null;
        private static string _endpoint;

        internal static HttpClient Client
        {
            get
            {
                if(_client == null)
                {
                    throw new NullReferenceException("Initialise API");
                }
                return _client;
            }
        }

        public static void Initialise(string endpoint, string apiKey)
        {
            _client = new HttpClient();
            _endpoint = endpoint;
            _client.DefaultRequestHeaders.Add("X-api-key", apiKey);
        }


        public static async Task<DataSource> GetDeviceAsync(string name)
        {
            string url = $"{_endpoint}/api/log/data/device?name={name}";
            using (var rs = await Client.GetAsync(url))
            {
                rs.EnsureSuccessStatusCode();

                var json = await rs.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<DataSource>(json);
            }
        }

        public static async Task PostDataAsync(List<DataPoint> data)
        {
            if (data.Any())
            {
                string url = $"{_endpoint}/api/log/data";
                var json = JsonConvert.SerializeObject(data);

                //Console.WriteLine(url + "\n" + json);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    var rs = await Client.PostAsync(url, content);
                    rs.EnsureSuccessStatusCode();
                }
            }
        }


        public static async Task PostMessageAsync(AgentMessage msg)
        {
            string url = $"{_endpoint}/api/agent/messages";

            using (var content = new StringContent(JsonConvert.SerializeObject(msg), Encoding.UTF8, "application/json"))
            {
                var rs = await Client.PostAsync(url, content);
                rs.EnsureSuccessStatusCode();
            }
        }
    }
}
