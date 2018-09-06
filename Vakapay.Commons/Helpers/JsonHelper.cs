namespace Vakapay.Commons.Helpers
{
    public class JsonHelper
    {
        public static T DeserializeObject<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        public static string SerializeObject<T>(T obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
    }
}