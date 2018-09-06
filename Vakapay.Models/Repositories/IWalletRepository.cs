using Vakapay.Models.Domains;
using Vakapay.Models.Entities;

namespace Vakapay.Models.Repositories
{
    public interface IWalletRepository
    {
        ReturnObject UpdateBalanceWallet(decimal amount, string Id, int version);
        Wallet FindWalletById(string Id);
        Wallet FindWalletByUserId(string userId);
        Wallet FindWalletBySql(string SqlString);
    }
}