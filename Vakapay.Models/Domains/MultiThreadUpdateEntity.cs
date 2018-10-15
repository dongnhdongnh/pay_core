using Vakapay.Commons.Helpers;

namespace Vakapay.Models.Domains
{
    public abstract class MultiThreadUpdateEntity
    {
        public string Id { get; set; } = CommonHelper.GenerateUuid();
        public int IsProcessing { get; set; }
        public int Version { get; set; }
        public string Status { get; set; }

        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
    }
}