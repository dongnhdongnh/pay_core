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
        List<BlockchainAddress> GetAddresses(string walletId, string networkName);
        List<BlockchainAddress> GetAddressesLimit(out int numberData,string walletId, string networkName,int skip, int take, string filter);
        Wallet FindByAddressAndNetworkName(string address, string networkName);
        BlockchainAddress GetAddressesInfo(string id, string networkName);
        List<string> GetStringAddresses(string walletId, string networkName);
        List<string> DistinctUserId();
    }
}