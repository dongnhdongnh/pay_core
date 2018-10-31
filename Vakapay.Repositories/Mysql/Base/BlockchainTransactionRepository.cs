using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Repositories.Mysql.Base
{
    public abstract class BlockchainTransactionRepository<TTransaction> :
        MultiThreadUpdateEntityRepository<TTransaction>,
        IRepositoryBlockchainTransaction<TTransaction> where TTransaction : BlockchainTransaction
    {
        public BlockchainTransactionRepository(string connectionString) : base(connectionString)
        {
        }

        public BlockchainTransactionRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }


        //
        //
        //		public BlockchainTransaction FindTransactionPending()
        //		{
        //			try
        //			{
        //				return FindTransactionByStatus(Status.STATUS_PENDING);
        //			}
        //			catch (Exception e)
        //			{
        //				throw;
        //			}
        //		}
        //
        //		public List<BlockchainTransaction> FindTransactionsPending()
        //		{
        //			try
        //			{
        //				return FindTransactionsByStatus(Status.STATUS_PENDING);
        //			}
        //			catch (Exception e)
        //			{
        //				throw;
        //			}
        //		}
        //
        //		public List<BlockchainTransaction> FindTransactionsInProcess()
        //		{
        //			try
        //			{
        //				if (Connection.State != ConnectionState.Open)
        //					Connection.Open();
        //
        //				var sqlString = $"Select * from {TableName} where InProcess = 1";
        //				var result = Connection.Query<TTransaction>(sqlString).ToList<BlockchainTransaction>();
        //				return result;
        //			}
        //			catch (Exception e)
        //			{
        //				throw;
        //			}
        //		}
        //
        //		public BlockchainTransaction FindTransactionError()
        //		{
        //			try
        //			{
        //				return FindTransactionByStatus(Status.STATUS_ERROR);
        //			}
        //			catch (Exception e)
        //			{
        //				throw;
        //			}
        //		}
        //
        //		public BlockchainTransaction FindTransactionByStatus(string status)
        //		{
        //			try
        //			{
        //				if (Connection.State != ConnectionState.Open)
        //					Connection.Open();
        //
        //				var sqlString = $"Select * from {TableName} where Status = @status and InProcess = 0";
        //				var result =
        //					Connection.QueryFirstOrDefault<TTransaction>(sqlString, new { status = status });
        //				return result;
        //			}
        //			catch (Exception e)
        //			{
        //				Console.WriteLine(e.ToString());
        //				throw;
        //			}
        //		}
        //
        //		public List<BlockchainTransaction> FindTransactionsByStatus(string status)
        //		{
        //			try
        //			{
        //				if (Connection.State != ConnectionState.Open)
        //					Connection.Open();
        //				//Console.WriteLine("FIND TRANSACTION BY STATUS");
        //				var sqlString = $"Select * from {TableName} where Status = @status and InProcess = 0";
        //				var result = Connection.Query<TTransaction>(sqlString, new { status = status })
        //					.ToList<BlockchainTransaction>();
        //				return result;
        //			}
        //			catch (Exception e)
        //			{
        //				throw;
        //			}
        //		}
        //
        //		public async Task<ReturnObject> LockForProcess(BlockchainTransaction transaction)
        //		{
        //			//Console.WriteLine("LockForProcess");
        //			int cache = transaction.Version;
        //			var _setQuery = new Dictionary<string, string>();
        //			_setQuery.Add(nameof(transaction.Version), (transaction.Version + 1).ToString());
        //			_setQuery.Add(nameof(transaction.IsProcessing), "1");
        //			var _updateQuery = new Dictionary<string, string>();
        //			_updateQuery.Add(nameof(transaction.Id), transaction.Id);
        //			_updateQuery.Add(nameof(transaction.Version), transaction.Version.ToString());
        //			_updateQuery.Add(nameof(transaction.IsProcessing), "0");
        //			if (cache != transaction.Version)
        //			{
        //				Console.WriteLine("fucking error");
        //			}
        //			return ExcuteSQL(SqlHelper.Query_Update(TableName, _setQuery, _updateQuery));
        //
        //			//try
        //			//{
        //			//	if (Connection.State != ConnectionState.Open)
        //			//		Connection.Open();
        //			//	var sqlCommand = "Update " + TableName +
        //			//						" Set Version = Version + 1, InProcess = 1 Where Id = @Id and Version = @Version and InProcess = 0";
        //
        //			//	var update = Connection.Execute(sqlCommand, new { Id = transaction.Id, Version = transaction.Version });
        //			//	if (update == 1)
        //			//	{
        //			//		return new ReturnObject
        //			//		{
        //			//			Status = Status.StatusSuccess,
        //			//			Message = "Update Success",
        //			//		};
        //			//	}
        //
        //			//	return new ReturnObject
        //			//	{
        //			//		Status = Status.StatusError,
        //			//		Message = "Update Fail",
        //			//	};
        //			//}
        //			//catch (Exception e)
        //			//{
        //			//	throw;
        //			//}
        //		}
        //
        //		public async Task<ReturnObject> ReleaseLock(BlockchainTransaction transaction)
        //		{
        //			//Console.WriteLine("ReleaseLock");
        //			int cache = transaction.Version;
        //			var _setQuery = new Dictionary<string, string>();
        //			_setQuery.Add(nameof(transaction.Version), (transaction.Version + 1).ToString());
        //			_setQuery.Add(nameof(transaction.IsProcessing), "0");
        //			var _updateQuery = new Dictionary<string, string>();
        //			_updateQuery.Add(nameof(transaction.Id), transaction.Id);
        //			_updateQuery.Add(nameof(transaction.Version), transaction.Version.ToString());
        //			_updateQuery.Add(nameof(transaction.IsProcessing), "1");
        //			if (cache != transaction.Version)
        //			{
        //				Console.WriteLine("fucking error");
        //			}
        //			return ExcuteSQL(SqlHelper.Query_Update(TableName, _setQuery, _updateQuery));
        //
        //
        //			//try
        //			//{
        //			//	if (Connection.State != ConnectionState.Open)
        //			//		Connection.Open();
        //			//	var sqlCommand = "Update " + TableName +
        //			//						" Set Version = Version + 1, InProcess = 0  Where Id = @Id and Version = @Version and InProcess = 1";
        //
        //			//	var update = Connection.Execute(sqlCommand, new { Id = transaction.Id, Version = transaction.Version });
        //			//	if (update == 1)
        //			//	{
        //			//		return new ReturnObject
        //			//		{
        //			//			Status = Status.StatusSuccess,
        //			//			Message = "Update Success",
        //			//		};
        //			//	}
        //
        //			//	return new ReturnObject
        //			//	{
        //			//		Status = Status.StatusError,
        //			//		Message = "Update Fail",
        //			//	};
        //			//}
        //			//catch (Exception e)
        //			//{
        //			//	throw;
        //			//}
        //		}
        //
        //		public async Task<ReturnObject> SafeUpdate(BlockchainTransaction transaction)
        //		{
        //			//Console.WriteLine("SafeUpdate");
        //			int cache = transaction.Version;
        //			var _setQuery = new Dictionary<string, string>();
        //			_setQuery.Add(nameof(transaction.Version), (transaction.Version + 1).ToString());
        //			_setQuery.Add(nameof(transaction.IsProcessing), "0");
        //			_setQuery.Add(nameof(transaction.Status), transaction.Status);
        //			_setQuery.Add(nameof(transaction.UpdatedAt), transaction.UpdatedAt.ToString());
        //			if (transaction.Hash != null)
        //			{
        //				_setQuery.Add(nameof(transaction.Hash), transaction.Hash.ToString());
        //			}
        //			var _updateQuery = new Dictionary<string, string>();
        //			_updateQuery.Add(nameof(transaction.Id), transaction.Id);
        //			_updateQuery.Add(nameof(transaction.Version), transaction.Version.ToString());
        //			_updateQuery.Add(nameof(transaction.IsProcessing), "1");
        //			if (cache != transaction.Version)
        //			{
        //				Console.WriteLine("fucking error");
        //			}
        //			return ExcuteSQL(SqlHelper.Query_Update(TableName, _setQuery, _updateQuery));
        //
        //			//try
        //			//{
        //			//	if (Connection.State != ConnectionState.Open)
        //			//		Connection.Open();
        //			//	var sqlCommand = "Update " + TableName +
        //			//						" Set Version = Version + 1, InProcess = 0, Status = @Status, UpdatedAt = @UpdatedAt,Hash = @Hash Where Id = @Id and Version = @Version and InProcess = 1";
        //
        //			//	var update = Connection.Execute(sqlCommand,
        //			//		new
        //			//		{
        //			//			Id = transaction.Id,
        //			//			Version = transaction.Version,
        //			//			Status = transaction.Status,
        //			//			UpdatedAt = transaction.UpdatedAt,
        //			//			Hash = transaction.Hash
        //			//		});
        //			//	if (update == 1)
        //			//	{
        //			//		return new ReturnObject
        //			//		{
        //			//			Status = Status.StatusSuccess,
        //			//			Message = "Update Success",
        //			//		};
        //			//	}
        //
        //			//	return new ReturnObject
        //			//	{
        //			//		Status = Status.StatusError,
        //			//		Message = "Update Fail",
        //			//	};
        //			//}
        //			//catch (Exception e)
        //			//{
        //			//	throw;
        //			//}
        //		}
        //
        public List<TTransaction> FindTransactionsNotCompleteOnNet()
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                //Console.WriteLine("FIND TRANSACTION BY STATUS");
                var sqlString =
                    $"Select * from {TableName} where BlockNumber = @BlockNumber and IsProcessing = 0 and Status=@Status";
                var result = Connection
                    .Query<TTransaction>(sqlString, new {BlockNumber = 0, Status = Status.STATUS_COMPLETED})
                    .ToList();
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<BlockchainTransaction> FindTransactionHistory(int offset = -1, int limit = -1,
            string[] orderBy = null)
        {
            try
            {
                var setQuery = new Dictionary<string, string>();
                setQuery.Add(nameof(BlockchainTransaction.Status), Status.STATUS_COMPLETED);
                return FindBySql(SqlHelper.Query_Search(TableName, setQuery, limit, offset, orderBy))
                    .ToList<BlockchainTransaction>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
                //	throw;
            }

            //	throw new NotImplementedException();
        }

        public List<BlockchainTransaction> FindTransactionsByUserId(string userId)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("FindTransactionsByUserId: UserId cannot be null or empty");
                }

                var sqlString = $"Select * from {TableName} where UserId = @UserId";
                var result = Connection.Query<TTransaction>(sqlString, new {UserId = userId})
                    .ToList<BlockchainTransaction>();
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<BlockchainTransaction> FindTransactionsFromAddress(string fromAddress)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                if (string.IsNullOrEmpty(fromAddress))
                {
                    return new List<BlockchainTransaction>();
                }

                var sqlString = $"Select * from {TableName} where FromAddress = @Address";
                var result = Connection.Query<TTransaction>(sqlString, new {Address = fromAddress})
                    .ToList<BlockchainTransaction>();
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<BlockchainTransaction> FindTransactionsToAddress(string toAddress)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                if (string.IsNullOrEmpty(toAddress))
                {
                    return new List<BlockchainTransaction>();
                }

                var sqlString = $"Select * from {TableName} where ToAddress = @Address";
                var result = Connection.Query<TTransaction>(sqlString, new {Address = toAddress})
                    .ToList<BlockchainTransaction>();
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public List<BlockchainTransaction> FindTransactionsInner(string innerAddress)
        {
            var className = GetClassName();

            if (className.Contains("Deposit"))
            {
                return FindTransactionsToAddress(innerAddress);
            }

            if (className.Contains("Withdraw"))
            {
                return FindTransactionsFromAddress(innerAddress);
            }

            throw new Exception(
                className + ": Transaction repository class name not contain \"Deposit\" or \"Withdraw\" keyword");
        }


        public List<BlockchainTransaction> FindTransactionHistoryAll(out int numberData, string userId, string currency,
            string tableNameWithdraw, string tableNameDeposit, string tableInternalWithdraw, int offset, int limit,
            string[] orderByValue, string whereValue)

        {
            numberData = -1;
            try
            {
                string searchString = "";
                if (whereValue != null && whereValue != String.Empty && whereValue != "")
                {
                    searchString = " and ABS(Amount)= "+ whereValue+" ";
                }

                tableInternalWithdraw = tableInternalWithdraw.Replace("`", string.Empty);
                var selectInternal =
                    $"SELECT {tableInternalWithdraw}.Id,SenderUserId as UserId,SenderUserId as FromAddress,Email as ToAddress,{tableInternalWithdraw}.CreatedAt,{tableInternalWithdraw}.Status,{tableInternalWithdraw}.Description,-Amount as Amount " +
                    $"FROM {tableInternalWithdraw} left join User on ReceiverUserId=User.Id" +
                    $" where SenderUserId = '{userId}'and Currency = '{currency}' " + searchString +
                    $"UNION ALL " +
                    $"SELECT {tableInternalWithdraw}.Id,ReceiverUserId as UserId,Email as FromAddress,ReceiverUserId as ToAddress,{tableInternalWithdraw}.CreatedAt,{tableInternalWithdraw}.Status,{tableInternalWithdraw}.Description,Amount as Amount " +
                    $"FROM {tableInternalWithdraw} left join User on SenderUserId=User.Id" +
                    $" where ReceiverUserId = '{userId}'and Currency = '{currency}'" + searchString;
                var selectThing = "Id,UserId,FromAddress,ToAddress,CreatedAt,Status,Description";
                var output =
                    $"Select * from ( SELECT {selectThing},-Amount AS Amount FROM {tableNameWithdraw} WHERE UserId='{userId}'" +
                    searchString +
                    $" UNION ALL " +
                    $" SELECT {selectThing},Amount FROM {tableNameDeposit} WHERE UserId='{userId}' {searchString} UNION ALL {selectInternal}) as t_uni ";
                var outputCount =
                    $"Select count(*) from ( SELECT {selectThing},-Amount AS Amount FROM {tableNameWithdraw} WHERE UserId='{userId}'" +
                    searchString +
                    $" UNION ALL " +
                    $" SELECT {selectThing},Amount FROM {tableNameDeposit} WHERE UserId='{userId}' {searchString} UNION ALL {selectInternal}) as t_uni ";
                numberData = ExcuteCount(outputCount);
                StringBuilder orderStr = new StringBuilder("");
                int count = 0;
                if (orderByValue != null)
                {
                    count = 0;
                    foreach (var prop in orderByValue)
                    {
                        //if (prop.Value != null)
                        {
                            
                            if (count > 0)
                                orderStr.Append(",");
                            if (prop[0].Equals('-'))
                            {
                                orderStr.AppendFormat(" {0} DESC ", prop.Remove(0,1));
                            }
                            else
                            {
                                orderStr.AppendFormat(" {0}", prop);
                            }
                            count++;
                        }
                    }

                    output += " ORDER BY " + orderStr.ToString();
                }

                if (limit > 0)
                {
                    output += " LIMIT " + limit;
                }

                if (offset > 0)
                {
                    output += " OFFSET " + offset;
                }

                return FindBySql(output).ToList<BlockchainTransaction>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
                //	throw;
            }
        }

        public string GetTableName()
        {
            return TableName;
            // throw new NotImplementedException();
        }

        public override Task<ReturnObject> SafeUpdate(TTransaction row)
        {
            return base.SafeUpdate(row, new[] {nameof(row.Hash)});
        }
    }
}