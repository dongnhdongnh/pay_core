namespace Vakapay.Models.Entities
{
    public class SmsQueue
    {
        public string Id { get; set; }
        public string TextSend { get; set; }
        public int Status { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }
        public int Version { get; set; }
    }
}
