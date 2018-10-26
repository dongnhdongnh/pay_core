namespace Vakapay.ApiServer.Models
{
    public class ResultApiAccess
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string KeyApi { get; set; }
        public string Permissions { get; set; }
        public string Wallets { get; set; }
        public int Status { get; set; }
    }
}