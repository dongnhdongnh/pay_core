using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Vakapay.Commons.Helpers
{
    public class HttpClientHelper
    {
        private static readonly HttpClient client = new HttpClient();

        private static async Task<string> _PostRequest(string url, Dictionary<string, string> values)
        {
            var content = JsonConvert.SerializeObject(values);
            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, httpContent);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        public static string PostRequest(string url, Dictionary<string, string> values)
        {
            return _PostRequest(url, values).Result;
        }
    }
}