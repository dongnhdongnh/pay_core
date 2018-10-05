using Dapper;
using System;
using System.Data;
using System.Threading.Tasks;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class VakacoinAccountRepository : MySqlBaseRepository<VakacoinAccount>, IVakacoinAccountRepository
    {
        public VakacoinAccountRepository(string connectionString) : base(connectionString)
        {
        }

        public VakacoinAccountRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public VakacoinAccount FindByAddress(string address) //FindByAccountName
        {
            return FindByAccountName(address);
        }

        public Task<ReturnObject> InsertAddress(string address, string walletId, string other)
        {
            throw new NotImplementedException();
        }

        public VakacoinAccount FindByAccountName(string accountName)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var sQuery = "SELECT * FROM " + TableName + " WHERE AccountName = @AC";
                var result = Connection.QuerySingleOrDefault<VakacoinAccount>(sQuery, new {AC = accountName});

                return result;
            }
            catch (Exception e)
            {
                Logger.Error("Find Vakacoin Account by AccountName Fail =>> fail: " + e.Message);
                throw;
            }
        }
    }
}