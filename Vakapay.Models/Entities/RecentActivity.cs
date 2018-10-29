namespace Vakapay.Models.Entities
{
    public class RecentActivity
    {
        public long TimeStamp { get; set; }
        public bool IsSend { get; set; }
        public string NetworkName { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string Hash { get; set; }
        public decimal Amount { get; set; }
        public decimal Value { get; set; }
        public decimal Price { get; set; }
        public int BockNumber { get; set; }
        public string Status { get; set; }
    }
}