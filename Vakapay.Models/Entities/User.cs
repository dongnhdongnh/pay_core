using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vakapay.Models.Entities
{
    [Table("user")]
    public class User
    {
        public User()
        {
        }
        public string Id { get; set; }
        public string Email { get; set; }
        public string SecondPassword { get; set; }
        public string IpWhiteList { get; set; }
        public int Status { get; set; }
    }
}
