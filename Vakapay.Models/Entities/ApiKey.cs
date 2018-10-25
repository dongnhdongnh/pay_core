namespace Vakapay.Models.Entities
{
    public class ApiKey
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Secret { get; set; }
        public string KeyApi { get; set; }
        public string CallbackUrl { get; set; }
        public string ApiAllow { get; set; }
        public string Permissions { get; set; }
        public string Wallets { get; set; }
        public int Status { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }
    }
}