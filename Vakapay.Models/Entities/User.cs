using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    [Table("User")]
    public class User : BaseModel
    {
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
        public string CurrencyKey { get; set; }
        public string TimezoneKey { get; set; }
        public string Notifications { get; set; }
        public string SecretAuthToken { get; set; }
        public int Verification { get; set; }
        public int IsLockScreen { get; set; }
        public bool TwoFactor { get; set; }
        public string TwoFactorSecret { get; set; }

        public static User FromJson(string json) =>
            JsonHelper.DeserializeObject<User>(json, JsonHelper.CONVERT_SETTINGS);

        public static string ToJson(User self) =>
            JsonHelper.SerializeObject(self, JsonHelper.CONVERT_SETTINGS);
    }
}