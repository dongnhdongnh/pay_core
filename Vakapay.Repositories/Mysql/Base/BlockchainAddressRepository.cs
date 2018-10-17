 using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Repositories.Mysql.Base
{
    public abstract class BlockchainAddressRepository<TAddress> : MySqlBaseRepository<TAddress>, IAddressRepository<TAddress>
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

        public abstract Task<ReturnObject> InsertAddress(string address, string walletId, string other);
    }
}