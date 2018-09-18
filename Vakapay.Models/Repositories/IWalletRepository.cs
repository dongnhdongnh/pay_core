using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IWalletRepository : IRepositoryBase<Wallet>
    {
        ReturnObject UpdateBalanceWallet(decimal amount, string Id, int version);
        ReturnObject UpdateBalanceWallet(decimal amount, string id, int version, string addr, string networkName);
        Wallet FindByAddress(string address, string networkName);
    }
}