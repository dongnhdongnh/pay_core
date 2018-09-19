using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vakapay.Models.Entities
{
    [Table("emailQueue")]
    public class EmailQueue
    {
        public string Id { get; set; }
        public string ToEmail { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
    }
}