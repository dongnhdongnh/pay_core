namespace Vakapay.Models.Entities
{
    public class UserActionLog
    {
        public string Id { get; set; }
        public string ActionName { get; set; }
        public string Description { get; set; }
        public string Ip { get; set; }
        public string UserId { get; set; }
        public int CreatedAt { get; set; }
    }
}