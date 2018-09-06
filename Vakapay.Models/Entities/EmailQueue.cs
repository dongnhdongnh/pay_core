using System;
namespace Vakapay.Models.Entities
{
    public class EmailQueue
    {
        public string Id { get; set; }
        public string ToEmail { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }


    }
}
