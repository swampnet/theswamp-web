using Newtonsoft.Json;
using System;
using System.Net.Http;
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


        public static async Task<Device> GetDeviceAsync(string name)
        {
            string url = $"{_endpoint}/api/monitor?deviceName={name}";
            using (var rs = await Client.GetAsync(url))
            {
                rs.EnsureSuccessStatusCode();

                var json = await rs.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Device>(json);
            }
        }
    }
}
