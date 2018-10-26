using Newtonsoft.Json;
using Vakapay.Commons.Helpers;

namespace Vakapay.ApiServer.Models
{
    public class DataApiKeyForm
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("wallets")] public string Wallets { get; set; }
        [JsonProperty("apis")] public string Apis { get; set; }
        [JsonProperty("notificationUrl")] public string NotificationUrl { get; set; }
        [JsonProperty("allowedIp")] public string AllowedIp { get; set; }

        public static DataApiKeyForm FromJson(string json) =>
            JsonHelper.DeserializeObject<DataApiKeyForm>(json);
    }
}