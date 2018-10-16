﻿using System;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Entities;
using Vakapay.Models.Entities.BTC;
using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vakapay.Models.Domains
{
    public abstract class BlockchainTransaction : MultiThreadUpdateEntity
    {
<<<<<<< HEAD
        public string Id
        {
            get
            {
                return CommonHelper.GenerateUuid();
            }
        }
=======
>>>>>>> master
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
                    return CryptoCurrency.VKC;
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
