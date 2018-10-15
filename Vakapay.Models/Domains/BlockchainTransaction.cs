using System;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Entities;
using Vakapay.Models.Entities.BTC;

namespace Vakapay.Models.Domains
{
    public abstract class BlockchainTransaction
    {
        public string Id { get; } = CommonHelper.GenerateUuid();
        public string UserId { get; set; }
        public string Hash { get; set; }
        public int BlockNumber { get; set; }
        public decimal Amount { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Fee { get; set; }
        public string Status { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public int IsProcessing { get; set; }
        public int Version { get; set; }

        public string NetworkName()
        {
            switch (GetType().Name)
            {
                case nameof(BitcoinDepositTransaction):
                case nameof(BitcoinWithdrawTransaction):
                case nameof(BitcoinTransaction):
                    return CryptoCurrency.BTC;
                case nameof(EthereumDepositTransaction):
                case nameof(EthereumWithdrawTransaction):
                case nameof(EthereumTransaction):
                    return CryptoCurrency.ETH;
                case nameof(VakacoinDepositTransaction):
                case nameof(VakacoinWithdrawTransaction):
                case nameof(VakacoinTransaction):
                    return CryptoCurrency.VKC;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
