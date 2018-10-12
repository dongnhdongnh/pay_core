using Newtonsoft.Json;

namespace Vakapay.Models.Domains
{
    [JsonObject]
    public class ReturnObject
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }

        public string ToJson() =>
            JsonHelper.SerializeObject(this, JsonHelper.ConvertSettings);

        public static ReturnObject FromJson(string json) =>
            JsonHelper.DeserializeObject<ReturnObject>(json, JsonHelper.ConvertSettings);
    }
}