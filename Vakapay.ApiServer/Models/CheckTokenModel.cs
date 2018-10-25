namespace Vakapay.ApiServer.Models
{
    public class CheckTokenModel
    {
        public string Secret { get; set; }
        public string NewSecret { get; set; }
    }
}