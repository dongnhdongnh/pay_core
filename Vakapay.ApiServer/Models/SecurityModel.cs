namespace Vakapay.ApiServer.Models
{
    public class SecurityModel
    {
        public bool isEnableTwofa { get; set; }
        public int twofaOption { get; set; }
    }
}