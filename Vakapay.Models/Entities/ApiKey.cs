namespace Vakapay.Models.Entities
{
    public class ApiKey
    {
        
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Secret { get; set; }
        public string CallbackUrl { get; set; }
    }
}
