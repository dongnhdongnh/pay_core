using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vakapay.Models.Entities
{
    [Table("EmailQueue")]
    public class EmailQueue
    {
        public string Id { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Template { get; set; } //value = "new device", "sent", "verify"
        
        //new device template
        public string DeviceLocation { get; set; }
        public string DeviceIP { get; set; }
        public string DeviceBrowser { get; set; }
        public string DeviceAuthorizeUrl { get; set; }
        
        // Sent/Received template
        public decimal Amount { get; set; }
        public string SentOrReceived { get; set; } //value = "sent" or "received"
        public string NetworkName { get; set; }
        
        //Verify email template
        public string VerifyUrl { get; set; }
        
        public string Status { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public int InProcess { get; set; }
        public int Version { get; set; }
    }
}