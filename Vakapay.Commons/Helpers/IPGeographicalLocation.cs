using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Vakapay.Commons.Helpers
{
    public partial class IpGeographicalLocation
    {
        [JsonProperty("ip")] public string Ip { get; set; }

        [JsonProperty("country_code")] public string CountryCode { get; set; }

        [JsonProperty("country_name")] public string CountryName { get; set; }

        [JsonProperty("region_code")] public string RegionCode { get; set; }

        [JsonProperty("region_name")] public string RegionName { get; set; }

        [JsonProperty("city")] public string City { get; set; }

        [JsonProperty("zip_code")] public string ZipCode { get; set; }

        [JsonProperty("time_zone")] public TimeZone TimeZone { get; set; }

        [JsonProperty("currency")] public Currency Currency { get; set; }

        [JsonProperty("latitude")] public string Latitude { get; set; }

        [JsonProperty("longitude")] public string Longitude { get; set; }

        [JsonProperty("metro_code")] public string MetroCode { get; set; }

        private IpGeographicalLocation()
        {
        }

        public static async Task<IpGeographicalLocation> QueryGeographicalLocationAsync(string ipAddress)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync("http://api.ipstack.com/" + ipAddress +
                                                     "?access_key=aa7359fbf9db81bc6e7c96078784cb0c");

            return JsonConvert.DeserializeObject<IpGeographicalLocation>(result);
        }
    }

    public class Currency
    {
        [JsonProperty("code")] public string Code { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("plural")] public string Plural { get; set; }
        [JsonProperty("symbol")] public string Symbol { get; set; }
        [JsonProperty("symbol_native")] public string SymbolNative { get; set; }
    }

    public class TimeZone
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("code")] public string Code { get; set; }
    }

    public partial class IpGeographicalLocation
    {
        public static IpGeographicalLocation FromJson(string json) =>
            JsonConvert.DeserializeObject<IpGeographicalLocation>(json
                , JsonHelper.CONVERT_SETTINGS);
    }

    public static class Serialize
    {
        public static string ToJson(this IpGeographicalLocation self) =>
            JsonConvert.SerializeObject(self, JsonHelper.CONVERT_SETTINGS);
    }
}