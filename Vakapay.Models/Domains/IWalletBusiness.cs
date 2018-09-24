using System;
using System.Collections.Generic;
using System.Text;
using Vakapay.Models.Entities;

namespace Vakapay.Models.Domains
{
	public interface IWalletBusiness
	{

		ReturnObject CreateNewWallet(User user, BlockchainNetwork blockchainNetwork);
		ReturnObject UpdateAddressForWallet(string walletId, string address);
		bool CheckExistedAddress(string toAddress);
		ReturnObject UpdateBalance(string toAddress, decimal transaValue, string eTH);
		bool CheckExistedAndUpdateByAddress(string to, decimal v1, string v2);
	}
}
