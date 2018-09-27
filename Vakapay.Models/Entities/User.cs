using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

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
        public string Phone { get; set; }
        public string Fullname { get; set; }
        public string SecondPassword { get; set; }
        public string IpWhiteList { get; set; }
        public string Status { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }

        public static User FromJson(string json) =>
            JsonConvert.DeserializeObject<User>(json, JsonHelper.ConvertSettings);
    }
}