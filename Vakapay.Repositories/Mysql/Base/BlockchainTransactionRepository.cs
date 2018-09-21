	using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using MySql.Data.MySqlClient;
    using Vakapay.Commons.Helpers;
    using Vakapay.Models.Domains;
    using Vakapay.Models.Repositories.Base;
    using Vakapay.Repositories.Mysql.Base;
    
    namespace Vakapay.Repositories.Mysql
    {
        public abstract class BlockchainTransactionRepository<TTransaction> : MySqlBaseRepository<TTransaction>,
            IRepositoryBlockchainTransaction<TTransaction> where TTransaction : BlockchainTransaction
        {
            public BlockchainTransactionRepository(string connectionString) : base(connectionString)
            {
            }
    
            public BlockchainTransactionRepository(IDbConnection dbConnection) : base(dbConnection)
            {
            }
    
    
    
    
            public BlockchainTransaction FindTransactionPending()
            {
                try
                {
                    return FindTransactionByStatus(Status.StatusPending);
                }
                catch (Exception e)
                {
                    throw;
                }
            }
    
            public List<BlockchainTransaction> FindTransactionsPending()
            {
                try
                {
                    return FindTransactionsByStatus(Status.StatusPending);
                }
                catch (Exception e)
                {
                    throw;
                }
            }
    
            public List<BlockchainTransaction> FindTransactionsInProcess()
            {
                try
                {
                    if (Connection.State != ConnectionState.Open)
                        Connection.Open();
    
                    var sqlString = $"Select * from {TableName} where InProcess = 1";
                    var result = Connection.Query<TTransaction>(sqlString).ToList<BlockchainTransaction>();
                    return result;
                }
                catch (Exception e)
                {
                    throw;
                }
            }
    
            public BlockchainTransaction FindTransactionError()
            {
                try
                {
                    return FindTransactionByStatus(Status.StatusError);
                }
                catch (Exception e)
                {
                    throw;
                }
            }
    
            public BlockchainTransaction FindTransactionByStatus(string status)
            {
                try
                {
                    if (Connection.State != ConnectionState.Open)
                        Connection.Open();
                    
                    var sqlString = $"Select * from {TableName} where Status = @status and InProcess = 0";
                    var result =
                        Connection.QueryFirstOrDefault<TTransaction>(sqlString, new { status = status });
                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    throw;
                }
            }
    
            public List<BlockchainTransaction> FindTransactionsByStatus(string status)
            {
                try
                {
                    if (Connection.State != ConnectionState.Open)
                        Connection.Open();
                    //Console.WriteLine("FIND TRANSACTION BY STATUS");
                    var sqlString = $"Select * from {TableName} where Status = @status and InProcess = 0";
                    var result = Connection.Query<TTransaction>(sqlString, new { status = status })
                        .ToList<BlockchainTransaction>();
                    return result;
                }
                catch (Exception e)
                {
                    throw;
                }
            }
    
            public async Task<ReturnObject> LockForProcess(BlockchainTransaction transaction)
            {
                //Console.WriteLine("LockForProcess");
                var _setQuery = new Dictionary<string, string>();
                _setQuery.Add(nameof(transaction.Version), (transaction.Version + 1).ToString());
                _setQuery.Add(nameof(transaction.InProcess), "1");
                var _updateQuery = new Dictionary<string, string>();
                _updateQuery.Add(nameof(transaction.Id), transaction.Id);
                _updateQuery.Add(nameof(transaction.Version), transaction.Version.ToString());
                _updateQuery.Add(nameof(transaction.InProcess), "0");
                return ExcuteSQL(SqlHelper.Query_Update(TableName, _setQuery, _updateQuery));
    
                //try
                //{
                //	if (Connection.State != ConnectionState.Open)
                //		Connection.Open();
                //	var sqlCommand = "Update " + TableName +
                //						" Set Version = Version + 1, InProcess = 1 Where Id = @Id and Version = @Version and InProcess = 0";
    
                //	var update = Connection.Execute(sqlCommand, new { Id = transaction.Id, Version = transaction.Version });
                //	if (update == 1)
                //	{
                //		return new ReturnObject
                //		{
                //			Status = Status.StatusSuccess,
                //			Message = "Update Success",
                //		};
                //	}
    
                //	return new ReturnObject
                //	{
                //		Status = Status.StatusError,
                //		Message = "Update Fail",
                //	};
                //}
                //catch (Exception e)
                //{
                //	throw;
                //}
            }
    
            public async Task<ReturnObject> ReleaseLock(BlockchainTransaction transaction)
            {
                //Console.WriteLine("ReleaseLock");
                var _setQuery = new Dictionary<string, string>();
                _setQuery.Add(nameof(transaction.Version), (transaction.Version + 1).ToString());
                _setQuery.Add(nameof(transaction.InProcess), "0");
                var _updateQuery = new Dictionary<string, string>();
                _updateQuery.Add(nameof(transaction.Id), transaction.Id);
                _updateQuery.Add(nameof(transaction.Version), transaction.Version.ToString());
                _updateQuery.Add(nameof(transaction.InProcess), "1");
                return ExcuteSQL(SqlHelper.Query_Update(TableName, _setQuery, _updateQuery));
    
    
                //try
                //{
                //	if (Connection.State != ConnectionState.Open)
                //		Connection.Open();
                //	var sqlCommand = "Update " + TableName +
                //						" Set Version = Version + 1, InProcess = 0  Where Id = @Id and Version = @Version and InProcess = 1";
    
                //	var update = Connection.Execute(sqlCommand, new { Id = transaction.Id, Version = transaction.Version });
                //	if (update == 1)
                //	{
                //		return new ReturnObject
                //		{
                //			Status = Status.StatusSuccess,
                //			Message = "Update Success",
                //		};
                //	}
    
                //	return new ReturnObject
                //	{
                //		Status = Status.StatusError,
                //		Message = "Update Fail",
                //	};
                //}
                //catch (Exception e)
                //{
                //	throw;
                //}
            }
    
            public async Task<ReturnObject> SafeUpdate(BlockchainTransaction transaction)
            {
                //Console.WriteLine("SafeUpdate");
                var _setQuery = new Dictionary<string, string>();
                _setQuery.Add(nameof(transaction.Version), (transaction.Version + 1).ToString());
                _setQuery.Add(nameof(transaction.InProcess), "0");
                _setQuery.Add(nameof(transaction.Status), transaction.Status);
                _setQuery.Add(nameof(transaction.UpdatedAt), transaction.UpdatedAt.ToString());
                if (transaction.Hash != null)
                {
                    _setQuery.Add(nameof(transaction.Hash), transaction.Hash.ToString());
                }
                var _updateQuery = new Dictionary<string, string>();
                _updateQuery.Add(nameof(transaction.Id), transaction.Id);
                _updateQuery.Add(nameof(transaction.Version), transaction.Version.ToString());
                _updateQuery.Add(nameof(transaction.InProcess), "1");
                return ExcuteSQL(SqlHelper.Query_Update(TableName, _setQuery, _updateQuery));
    
                //try
                //{
                //	if (Connection.State != ConnectionState.Open)
                //		Connection.Open();
                //	var sqlCommand = "Update " + TableName +
                //						" Set Version = Version + 1, InProcess = 0, Status = @Status, UpdatedAt = @UpdatedAt,Hash = @Hash Where Id = @Id and Version = @Version and InProcess = 1";
    
                //	var update = Connection.Execute(sqlCommand,
                //		new
                //		{
                //			Id = transaction.Id,
                //			Version = transaction.Version,
                //			Status = transaction.Status,
                //			UpdatedAt = transaction.UpdatedAt,
                //			Hash = transaction.Hash
                //		});
                //	if (update == 1)
                //	{
                //		return new ReturnObject
                //		{
                //			Status = Status.StatusSuccess,
                //			Message = "Update Success",
                //		};
                //	}
    
                //	return new ReturnObject
                //	{
                //		Status = Status.StatusError,
                //		Message = "Update Fail",
                //	};
                //}
                //catch (Exception e)
                //{
                //	throw;
                //}
            }
        
    public List<BlockchainTransaction> FindTransactionsNotCompleteOnNet()
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
				//Console.WriteLine("FIND TRANSACTION BY STATUS");
				var sqlString = $"Select * from {TableName} where BlockNumber = @BlockNumber and InProcess = 0 and Status=@Status";
				var result = Connection.Query<TTransaction>(sqlString, new { BlockNumber = 0, Status = Status.StatusCompleted })
					.ToList<BlockchainTransaction>();
				return result;
			}
			catch (Exception e)
			{
				throw e;
			}
		}
	}
}