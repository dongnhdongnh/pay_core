﻿using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities.BTC
{
    public class BitcoinTransaction : BlockchainTransaction
    {
        public string BlockHash { get; set; }
    }

    [Table("BitcoinDepositTransaction")]
    public class BitcoinDepositTransaction : BitcoinTransaction
    {
        public static BitcoinDepositTransaction FromJson(string json) =>
            JsonHelper.DeserializeObject<BitcoinDepositTransaction>(json, JsonHelper.CONVERT_SETTINGS);

//        public string ToJson() =>
//            JsonHelper.SerializeObject(this, JsonHelper.ConvertSettings);
    }

    [Table("BitcoinWithdrawTransaction")]
    public class BitcoinWithdrawTransaction : BitcoinTransaction
    {
    }
}