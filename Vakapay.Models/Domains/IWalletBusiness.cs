using Vakapay.Models.Entities;

namespace Vakapay.Models.Domains
{
	public interface IWalletBusiness
	{

		ReturnObject CreateNewWallet(User user, string blockchainNetwork);
//		ReturnObject UpdateAddressForWallet(string walletId, string address);
		bool CheckExistedAddress(string address, string networkName);
		ReturnObject UpdateBalance(string toAddress, decimal transaValue, string networkName);
		bool CheckExistedAndUpdateByAddress(string to, decimal v1, string v2);
		ReturnObject MakeAllWalletForNewUser(User newUser);
		string FindEmailByAddressAndNetworkName(string addr, string networkName);
		ReturnObject SetHasAddressForWallet(string walletId);
	}
}
