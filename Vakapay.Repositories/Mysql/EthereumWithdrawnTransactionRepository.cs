using System.Data;
using Vakapay.Models.Entities.ETH;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class EthereumWithdrawnTransactionRepository : BlockchainTransactionRepository<EthereumWithdrawTransaction>,
        IEthereumWithdrawTransactionRepository
    {
        public EthereumWithdrawnTransactionRepository(string connectionString) : base(connectionString)
        {
        }

        public EthereumWithdrawnTransactionRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        //public ReturnObject ExcuteSQL(string sqlString, object transaction = null)
        //{
        //	try
        //	{
        //		if (Connection.State != ConnectionState.Open)
        //			Connection.Open();


        //		var result = 0;
        //		if (transaction != null)
        //		{
        //			MySqlTransaction _transaction = (MySqlTransaction)transaction;
        //			result = Connection.Execute(sqlString, null, _transaction);
        //		}
        //		else
        //		{
        //			result = Connection.Execute(sqlString);
        //		}

        //		var status = result > 0 ? Status.StatusSuccess : Status.StatusError;
        //		Console.WriteLine("Excute thing " + result);
        //		return new ReturnObject
        //		{
        //			Status = status,
        //			Message = status == Status.StatusError ? "Cannot Excute" : "Excute Success"
        //		};
        //	}
        //	catch (Exception e)
        //	{
        //		return new ReturnObject
        //		{
        //			Status = Status.StatusError,
        //			Message = e.Message
        //		};
        //	}
        //}

        //public object GetTransaction()
        //{
        //	if (Connection.State != ConnectionState.Open)
        //		Connection.Open();
        //	return Connection.BeginTransaction();
        //}


        //public void TransactionCommit(object transaction)
        //{
        //	MySqlTransaction _transaction = (MySqlTransaction)transaction;
        //	_transaction.Commit();
        //}

        //public void TransactionRollback(object transaction)
        //{
        //	MySqlTransaction _transaction = (MySqlTransaction)transaction;
        //	_transaction.Rollback();
        //}
    }
}