using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Repositories.Mysql.Base
{
    public abstract class BlockchainAddressRepository<TAddress> : MySqlBaseRepository<TAddress>,
        IAddressRepository<TAddress>
        where TAddress : BlockchainAddress
    {
        public BlockchainAddressRepository(string connectionString) : base(connectionString)
        {
        }

        public BlockchainAddressRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public TAddress FindByAddress(string address)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var sQuery = $"SELECT * FROM {TableName} WHERE Address = @AD";
                var result = Connection.QuerySingleOrDefault<TAddress>(sQuery, new {AD = address});

                return result;
            }
            catch (Exception e)
            {
                Logger.Error($"Find {typeof(TAddress).Name} Fail =>> fail: " + e.Message);
                throw;
            }
        }

        public List<TAddress> FindByWalletId(string walletId)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var sQuery = $"SELECT * FROM {TableName} WHERE WalletId = @WI";
                var result = Connection.Query<TAddress>(sQuery, new {WI = walletId}).ToList();

                return result;
            }
            catch (Exception e)
            {
                Logger.Error($"FindByWalletId walletID ({walletId}) Fail =>> fail: " + e.Message);
                throw;
            }
        }

        public List<TAddress> FindByWalletIdLimit(out int numberData, string walletId, int skip, int take,
            string filter)
        {
            numberData = -1;
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var sQuery = $"SELECT * FROM {TableName} WHERE WalletId = @WI";
                if (!string.IsNullOrEmpty(filter))
                {
                    sQuery += " AND Address LIKE '%" + filter + "%'";
                }

                numberData = Connection.Query(sQuery, new {WI = walletId}).Count();
                var result = Connection.Query<TAddress>(sQuery, new {WI = walletId}).Skip(skip).Take(take).ToList();

                return result;
            }
            catch (Exception e)
            {
                Logger.Error($"FindByWalletId walletID ({walletId}) Fail =>> fail: " + e.Message);
                throw;
            }
        }

        public List<TAddress> FindByUserIdAndCurrency(string userId, string currency)
        {
            try
            {
                var sQuery = $"SELECT t1.* FROM " + TableName +
                             " t1 INNER JOIN wallet t2 ON t1.WalletId = t2.Id WHERE t2.UserId = '" + userId +
                             "' AND t2.Currency = '" + currency + "';";
                var result = Connection.Query<TAddress>(sQuery);
                return result.ToList();
            }
            catch (Exception e)
            {
                Logger.Error("BitcoinDepositTransactioRepository =>> FindByUserIdAndCurrency fail: " + e.Message);
                return null;
            }
        }

        public abstract Task<ReturnObject> InsertAddress(string address, string walletId, string other);
    }
}