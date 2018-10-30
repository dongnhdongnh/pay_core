using System.Collections.Generic;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IWalletRepository : IRepositoryBase<Wallet>, IMultiThreadUpdateEntityRepository<Wallet>
    {
        ReturnObject UpdateBalanceWallet(decimal amount, string id, int version);
        List<Wallet> FindAllWalletByUser(User user);
        Wallet FindByUserAndNetwork(string userId, string currency);
        List<Wallet> FindNullAddress();
        Wallet FindByAddressAndNetworkName(string address, string networkName);
        List<string> GetStringAddresses(string walletId, string networkName);
        List<string> DistinctUserId();
    }
}