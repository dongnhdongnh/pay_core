using Vakapay.Models.Entities;

namespace Vakapay.Models.Domains
{
    public abstract class BlockchainTransaction
    {
        public string Id { get; set; }
        public string Hash { get; set; }
        public int BlockNumber { get; set; }
        public decimal Amount { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Fee { get; set; }
        public string Status { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public int InProcess { get; set; }
        public int Version { get; set; }

        public string NetworkName()
        {
            var networkName = "";
            var type = this.GetType();
            
            if (type == typeof(BitcoinDepositTransaction) || type == typeof(BitcoinWithdrawTransaction))
            {
                networkName = Domains.NetworkName.BTC;
            }
            else if (type == typeof(EthereumDepositTransaction) || type == typeof(EthereumWithdrawTransaction))
            {
                networkName = Domains.NetworkName.ETH;
            }
            else if (type == typeof(VakacoinDepositTransaction) || type == typeof(VakacoinWithdrawTransaction) ||
                     type == typeof(VakacoinTransaction))
            {
                networkName = Domains.NetworkName.VAKA;
            }

            return networkName;
        }
    }
}
