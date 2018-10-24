using Vakapay.Models.Entities;

namespace Vakapay.ApiAccess.Model
{
    public class FilterModel
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public User UserModel { get; set; }
        public ApiKey ApiKeyModel { get; set; }
    }
}