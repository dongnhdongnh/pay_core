using System;
using System.Data;
using System.Threading.Tasks;
using Vakapay.Commons.Constants;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities.BTC;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class BitcoinAddressRepository : BlockchainAddressRepository<BitcoinAddress>, IBitcoinAddressRepository
    {
        public BitcoinAddressRepository(string connectionString) : base(connectionString)
        {
        }

        public BitcoinAddressRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }


        public override async Task<ReturnObject> InsertAddress(string address, string walletId, string account = "")
        {
            try
            {
                var bcAddress = new BitcoinAddress
                {
                    Address = address,
                    Status = Status.STATUS_ACTIVE,
                    WalletId = walletId,
                };

                return Insert(bcAddress);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}