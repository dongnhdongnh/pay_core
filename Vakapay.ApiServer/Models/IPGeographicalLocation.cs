using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Vakapay.ApiServer.Models
{
    public class IPGeographicalLocation
    {
        [JsonProperty("ip")] public string IP { get; set; }

        [JsonProperty("country_code")] public string CountryCode { get; set; }

        [JsonProperty("country_name")] public string CountryName { get; set; }

        [JsonProperty("region_code")] public string RegionCode { get; set; }

        [JsonProperty("region_name")] public string RegionName { get; set; }

        [JsonProperty("city")] public string City { get; set; }

        [JsonProperty("zip_code")] public string ZipCode { get; set; }

        [JsonProperty("time_zone")] public string TimeZone { get; set; }

        [JsonProperty("latitude")] public float Latitude { get; set; }

        [JsonProperty("longitude")] public float Longitude { get; set; }

        [JsonProperty("metro_code")] public int MetroCode { get; set; }

        private IPGeographicalLocation()
        {
        }

        public static async Task<IPGeographicalLocation> QueryGeographicalLocationAsync(string ipAddress)
        {
            HttpClient client = new HttpClient();
            string result =
                await client.GetStringAsync("http://api.ipstack.com/" + ipAddress +
                                            "?access_key=aa7359fbf9db81bc6e7c96078784cb0c");

            return JsonConvert.DeserializeObject<IPGeographicalLocation>(result);
        }
    }
}