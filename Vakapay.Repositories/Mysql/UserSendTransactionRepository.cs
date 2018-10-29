using System;
using System.Data;
using Dapper;
using Vakapay.Models.Entities;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class UserSendTransactionRepository : MySqlBaseRepository<UserSendTransaction>
    {
        public UserSendTransactionRepository(string connectionString) : base(connectionString)
        {
        }

        public UserSendTransactionRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public UserSendTransaction FindExistedIdem(UserSendTransaction sendTransaction)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var sQuery =
                    $"SELECT * FROM {TableName} WHERE {nameof(sendTransaction.UserId)} = @UserId AND {nameof(sendTransaction.Idem)} = @Idem";

                var result = Connection.QuerySingleOrDefault<UserSendTransaction>(sQuery,
                    new {sendTransaction.UserId, sendTransaction.Idem});

                return result;
            }
            catch (Exception e)
            {
                Logger.Error("UserRepository =>> FindByEmailAddress fail: " + e.Message);
                throw;
            }
        }
    }
}