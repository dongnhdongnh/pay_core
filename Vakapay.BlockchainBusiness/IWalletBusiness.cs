using Vakapay.Models.Domains;
using Vakapay.Models.Entities;

namespace Vakapay.BlockchainBusiness
{
    public interface IWalletBusiness
    {
        ReturnObject CreateNewWallet(User user, string blockchainNetwork);
        bool CheckExistedAddress(string toAddress, string networkName);
        ReturnObject UpdateBalanceDeposit(string toAddress, decimal addedBalance, string networkName);
        bool CheckExistedAndUpdateByAddress(string to, decimal v1, string v2);
        ReturnObject MakeAllWalletForNewUser(User newUser);
        string FindEmailByAddressAndNetworkName(string addr, string networkName);
    }
}