using System.Threading.Tasks;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Repositories.Base
{
    public interface IRepositoryAddress
    {
        Task<ReturnObject> InsertAddress(string walletId, string other);
    }
}