namespace Vakapay.ApiServer.Models
{
    public class DataApiKeyForm
    {
        public string id { get; set; }
        public string wallets { get; set; }
        public string apis { get; set; }
        public string notificationUrl { get; set; }
        public string allowedIp { get; set; }
        
        public static DataApiKeyForm FromJson(string json) =>
            JsonHelper.DeserializeObject<DataApiKeyForm>(json);
    }
}