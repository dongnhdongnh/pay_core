using System.Collections.Generic;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
	public interface IWalletRepository : IRepositoryBase<Wallet>, IMultiThreadUpdateEntityRepository<Wallet>
	{
		ReturnObject UpdateBalanceWallet(decimal amount, string Id, int version);
//		Wallet FindByAddress(string address);
//		List<Wallet> FindByAddressAndNetworkName(string address, string networkName);
		List<Wallet> FindAllWalletByUser(User user);
		Wallet FindByUserAndNetwork(string userId, string networkName);
		List<Wallet> FindNullAddress();
		Wallet FindByAddressAndNetworkName(string address, string networkName);
		List<string> GetAddresses(string walletId, string networkName);
	}
}