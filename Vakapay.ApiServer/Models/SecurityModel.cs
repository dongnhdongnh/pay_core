using Newtonsoft.Json;

namespace Vakapay.ApiServer.Models
{
    public class SecurityModel
    {
        [JsonProperty("isEnableTwofa")] public bool IsEnableTwofa { get; set; }
        [JsonProperty("twofaOption")] public int TwofaOption { get; set; }
    }
}