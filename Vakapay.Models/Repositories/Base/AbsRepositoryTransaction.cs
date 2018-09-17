using System.Threading.Tasks;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Repositories.Base
{
    public abstract class AbsRepositoryTransaction
    {
        public async Task<ReturnObject> LockForProcess(string Id, int version)
        {
            return null;
        }

        public async Task<ReturnObject> ReleaseLock(string Id)
        {
            return null;
        }
    }
}