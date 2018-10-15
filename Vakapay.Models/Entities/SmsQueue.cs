namespace Vakapay.Models.Entities
{
    public class SmsQueue
    {
        public string Id { get; set; }
        public string TextSend { get; set; }
        public string To { get; set; }
        public string Status { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }
        public int Version { get; set; }
        public int InProcess { get; set; }
    }
}
