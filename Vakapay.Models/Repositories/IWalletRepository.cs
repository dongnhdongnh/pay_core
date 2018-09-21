using System.Collections.Generic;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IWalletRepository : IRepositoryBase<Wallet>
    {
        ReturnObject UpdateBalanceWallet(decimal amount, string Id, int version);
        Wallet FindByAddress(string address);
        List<Wallet> FindByAddressAndNetworkName(string address, string networkName);
    }
}