namespace Vakapay.Models.Domains
{
    public class EmailConfig
    {
        public const string VakapayUrl = "google.com.vn";

        public const string LogoImgUrl = "/images/logo.svg";

        public const string MailImgUrl = "/images/mail.svg";
        public const string HrImgUrl = "/images/hr.png";
        public const string DeviceImgUrl = "/images/device.png";

        // template
        public const string Template_NewDevice = "newDevice";
        public const string Template_Verify = "verify";
        public const string Template_SentOrReceived = "sent";
        
        // for sent template
        public const string SignInUrl = "google.com.vn";
        public const string SentOrReceived_Sent = "sent";
        public const string SentOrReceived_Received = "received";
        
        //email Subject
        //new device subject
        public const string Subject_NewDevice = "New device";
        //sent subject
        public const string Subject_SentOrReceived = "Balance notification!";
        //verify subject
        public const string Subject_Verify = "Verify account";
    }
}