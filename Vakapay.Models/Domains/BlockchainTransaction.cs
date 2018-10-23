using System;
using Vakapay.Commons.Constants;
using Vakapay.Models.Entities;
using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Commons.Helpers;

namespace Vakapay.Models.Domains
{
    public abstract class BlockchainTransaction : MultiThreadUpdateEntity
    {
//        public string Id => CommonHelper.GenerateUuid(); //existed in MultiThreadUpdateEntity

        public string UserId { get; set; }
        public string Hash { get; set; }
        public int BlockNumber { get; set; }
        public decimal Amount { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Fee { get; set; }


        //[Write(false)]
        //[Computed]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public int Type { get; set; }


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
                    return CryptoCurrency.VAKA;
                default:
                    throw new NotImplementedException();
            }
        }

        //public class PersonMapper : ClassMapper<BlockchainTransaction>
        //{
        //    public PersonMapper()
        //    {
        //      //  Table("Person");
        //        Map(m => m.Type).Ignore();
        //        AutoMap();
        //    }
        //}
    }
}
