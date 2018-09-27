using Newtonsoft.Json;

namespace Vakapay.Models.Domains
{
    public class ReturnObject
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }

        public static string ToJson(ReturnObject self) =>
            JsonConvert.SerializeObject(self, JsonHelper.ConvertSettings);

        public static ReturnObject FromJson(string json) =>
            JsonConvert.DeserializeObject<ReturnObject>(json, JsonHelper.ConvertSettings);
    }
}