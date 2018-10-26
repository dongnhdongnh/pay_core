using Vakapay.Commons.Helpers;

namespace Vakapay.ApiServer.Models
{
    public class ActionCode
    {
        public string TwofaEnable { get; set; }
        public string TwofaDisable { get; set; }
        public string UpdateOptionVerification { get; set; }
        public string SendTransaction { get; set; }
        public string ApiAccessAdd { get; set; }
        public string ApiAccessEdit { get; set; }
        public string ApiAccess { get; set; }
        public string ApiAccessDelete { get; set; }
        public string LockScreen { get; set; }

        public static ActionCode FromJson(string json) =>
            JsonHelper.DeserializeObject<ActionCode>(json);

        public static string ToJson(ActionCode self) =>
            JsonHelper.SerializeObject(self);
    }
}