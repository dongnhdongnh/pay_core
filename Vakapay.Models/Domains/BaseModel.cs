using Vakapay.Commons.Helpers;

namespace Vakapay.Models.Domains
{
    public class BaseModel
    {
        public string Id { get; set; } = CommonHelper.GenerateUuid();
        public long CreatedAt { get; set; } = CommonHelper.GetUnixTimestamp();
        public long UpdatedAt { get; set; } = CommonHelper.GetUnixTimestamp();
    }
}