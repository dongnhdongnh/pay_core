using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    public class InternalWithdrawTransaction : MultiThreadUpdateEntity
    {
        public string SenderUserId { get; set; }
        public string ReceiverUserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Idem { get; set; }
    }
}