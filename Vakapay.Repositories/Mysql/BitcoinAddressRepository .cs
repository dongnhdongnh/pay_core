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
//
//        public ReturnObject Update(BitcoinAddress objectUpdate)
//        {
//            throw new System.NotImplementedException();
//        }
//
//        public ReturnObject Delete(string Id)
//        {
//            throw new System.NotImplementedException();
//        }
//
//
//        private const string TableName = "bitcoinaddress";
//
//        public ReturnObject Insert(BitcoinAddress objectInsert)
//        {
//            try
//            {
//                if (Connection.State != ConnectionState.Open)
//                    Connection.Open();
//
//
//                var result = Connection.InsertTask<string, BitcoinAddress>(objectInsert);
//                var status = !String.IsNullOrEmpty(result) ? Status.StatusSuccess : Status.StatusError;
//                return new ReturnObject
//                {
//                    Status = status,
//                    Message = status == Status.StatusError ? "Cannot insert" : "Insert Success",
//                    Data = result
//                };
//            }
//            catch (Exception e)
//            {
//                throw e;
//            }
//        }
//
//        public BitcoinAddress FindById(string Id)
//        {
//            try
//            {
//                if (Connection.State != ConnectionState.Open)
//                    Connection.Open();
//
//                string sQuery = "SELECT * FROM bitcoinaddress WHERE Id = @ID";
//
//                var result = Connection.QuerySingleOrDefault<BitcoinAddress>(sQuery, new {ID = Id});
//
//
//                return result;
//            }
//            catch (Exception e)
//            {
//                throw e;
//            }
//        }
//
//        public List<BitcoinAddress> FindBySql(string sqlString)
//        {
//            try
//            {
//                if (Connection.State != ConnectionState.Open)
//                    Connection.Open();
//
//                var result = Connection.Query<BitcoinAddress>(sqlString);
//
//                return result.ToList();
//            }
//            catch (Exception e)
//            {
//                throw e;
//            }
//        }

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

//        public BitcoinAddress FindByAddress(string address)
//        {
//            throw new NotImplementedException();
//        }
    }
}