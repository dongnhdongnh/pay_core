using Newtonsoft.Json;

namespace Vakapay.ApiServer.Models
{
    public class Response
    {
        [JsonProperty(PropertyName = "data")] public object Data;
    }
}