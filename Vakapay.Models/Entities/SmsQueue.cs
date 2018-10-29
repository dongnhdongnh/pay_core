using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    public class SmsQueue : MultiThreadUpdateModel
    {
        public string TextSend { get; set; }
        public string To { get; set; }
    }
}