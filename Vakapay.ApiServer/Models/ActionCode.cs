namespace Vakapay.ApiServer.Models
{
    public class ActionCode
    {
        public string TwofaEnable { get; set; }
        public string TwofaDisable { get; set; }
        public string UpdateOptionVerification { get; set; }

        public string LockScreen { get; set; }


        public static ActionCode FromJson(string json) =>
            JsonHelper.DeserializeObject<ActionCode>(json);

        public static string ToJson(ActionCode self) =>
            JsonHelper.SerializeObject(self);
    }
}