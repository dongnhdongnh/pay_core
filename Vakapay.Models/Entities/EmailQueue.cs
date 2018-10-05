using System.ComponentModel.DataAnnotations.Schema;

namespace Vakapay.Models.Entities
{
    public enum EmailTemplate
    {
        NewDevice, Sent, Received, Verify
    }

    [Table("EmailQueue")]
    public class EmailQueue
    {
        public string Id { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public EmailTemplate Template { get; set; }

        //new device template
        public string DeviceLocation { get; set; }
        public string DeviceIP { get; set; }
        public string DeviceBrowser { get; set; }
        public string DeviceAuthorizeUrl { get; set; }

        // Sent/Received template
        public string SignInUrl { get; set; }
        public decimal Amount { get; set; }
        public string NetworkName { get; set; }
        public string TransactionId { get; set; }

        //Verify email template
        public string VerifyUrl { get; set; }

        public string Status { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public int InProcess { get; set; }
        public int Version { get; set; }

        public string GetAmount()
        {
            if (NetworkName == Domains.NetworkName.VAKA)
            {
                return Amount.ToString("N4") + " " + NetworkName;
            }
            else
            {
                return Amount + " " + NetworkName;
            }
        }
    }
}