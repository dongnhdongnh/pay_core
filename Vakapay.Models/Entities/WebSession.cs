using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Vakapay.Models.Entities
{
    [Table("WebSession")]
    public class WebSession
    {
        public string Id { get; set; }
        public bool Current { get; set; }
        public string Browser { get; set; }
        public string Ip { get; set; }
        public string Location { get; set; }
        public string UserId { get; set; }
        public int SignedIn { get; set; }
    }
}