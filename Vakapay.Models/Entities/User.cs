using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Vakapay.Models.Entities
{
    [Table("User")]
    public class User
    {
        public User()
        {
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Birthday { get; set; }
        public string CountryCode { get; set; }
        public string Avatar { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string StreetAddress1 { get; set; }
        public string StreetAddress2 { get; set; }
        public string PostalCode { get; set; }
        public string SecondPassword { get; set; }
        public string IpWhiteList { get; set; }
        public string Status { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }
        public string CurrencyKey { get; set; }
        public string TimezoneKey { get; set; }
        public string Notifications { get; set; }

        public static User FromJson(string json) =>
            JsonConvert.DeserializeObject<User>(json, JsonHelper.ConvertSettings);
        public static string ToJson(User self) =>
            JsonConvert.SerializeObject(self, JsonHelper.ConvertSettings);
    }
}