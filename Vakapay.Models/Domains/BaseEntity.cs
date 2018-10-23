using Vakapay.Commons.Helpers;

namespace Vakapay.Models.Entities
{
    public class BaseEntity
    {
        public string Id { get; set; } = CommonHelper.GenerateUuid();
        public long CreatedAt { get; set; } = CommonHelper.GetUnixTimestamp();
        public long UpdatedAt { get; set; } = CommonHelper.GetUnixTimestamp();
    }
}