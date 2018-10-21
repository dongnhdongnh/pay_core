using System.Collections.Generic;
using Vakapay.Commons.Constants;
using Vakapay.Models.Entities;

namespace Vakapay.Models.Domains
{
    public class EmailConfig
    {
        public const string VakapayUrl = "google.com.vn";

        public const string LogoImgUrl = "https://i.imgur.com/ooQLCzZ.png";
        public const string CheckImgUrl = "https://i.imgur.com/EEDBk5M.png";
        public const string MailImgUrl = "https://i.imgur.com/8idVPQD.png";
        public const string HrImgUrl = "https://i.imgur.com/SDIW5OD.png";
        public const string DeviceImgUrl = "https://i.imgur.com/2p0MC3f.png";
        public const long BtcConfirmations = 3;
        public const long EthConfirmations = 50;
        public const long VakaConfirmations = 15;
        
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

        public static readonly Dictionary<EmailTemplate, string> TemplateFiles = new Dictionary<EmailTemplate, string>()
        {
            {EmailTemplate.NewDevice, "newDevice.htm"},
            {EmailTemplate.Sent, "sent.htm"},
            {EmailTemplate.Received, "received.htm"},
            {EmailTemplate.Verify, "verify.htm"},
        };

        public static string GetNumberOfNeededConfirmation(string networkName)
        {
            long confirmation = 0;
            switch (networkName)
            {
                case CryptoCurrency.VAKA:
                    confirmation = EmailConfig.VakaConfirmations;
                    break;
                case CryptoCurrency.ETH:
                    confirmation = EmailConfig.EthConfirmations;
                    break;
                case CryptoCurrency.BTC:
                    confirmation = EmailConfig.BtcConfirmations;
                    break;
            }

            return confirmation.ToString();
        }
    }
}