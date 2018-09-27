using Newtonsoft.Json;

namespace Vakaxa.VakaxaIdAPI.Model
{
    public partial class UserModel
    {
        [JsonProperty("Email")] public string Email { get; set; }

        [JsonProperty("Phone")] public string Phone { get; set; }

        [JsonProperty("Fullname")] public string Fullname { get; set; }
    }

    public partial class UserModel
    {
        public static UserModel FromJson(string json) =>
            JsonConvert.DeserializeObject<UserModel>(json, JsonHelper.ConvertSettings);
    }

    public static class Serialize
    {
        public static string ToJson(this UserModel self) =>
            JsonConvert.SerializeObject(self, JsonHelper.ConvertSettings);
    }
}