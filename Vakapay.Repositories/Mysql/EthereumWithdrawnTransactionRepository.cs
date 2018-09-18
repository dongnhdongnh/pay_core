using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{

	public class EthereumWithdrawnTransactionRepository : MysqlBaseConnection, IEthereumWithdrawTransactionRepository
	{
		String tableName = "vakapay.ethereumwithdrawtransaction";
		public string Query_Search(Dictionary<string, string> whereValue)
		{
			StringBuilder whereStr = new StringBuilder("");
			int count = 0;
			foreach (var prop in whereValue)
			{
				if (prop.Value != null)
				{
					if (count > 0)
						whereStr.Append(" AND ");
					whereStr.AppendFormat(" {0}='{1}'", prop.Key, prop.Value);
					count++;
				}
			}

			string output = string.Format("SELECT * FROM {0} WHERE {1}", tableName, whereStr);
			Console.WriteLine(output);
			return output;
		}

		public string Query_Update(object updateValue, Dictionary<string, string> whereValue)
		{
			StringBuilder updateStr = new StringBuilder("");
			StringBuilder whereStr = new StringBuilder("");

			int count = 0;
			foreach (PropertyInfo prop in updateValue.GetType().GetProperties())
			{
				if (prop.GetValue(updateValue, null) != null)
				{
					if (count > 0)
						updateStr.Append(",");
					updateStr.AppendFormat(" {0}='{1}'", prop.Name, prop.GetValue(updateValue, null));
					count++;
				}
			}

			// if (whereStr != null)
			count = 0;
			foreach (var prop in whereValue)
			{
				if (prop.Value != null)
				{
					if (count > 0)
						whereStr.Append(" AND ");
					whereStr.AppendFormat(" {0}='{1}'", prop.Key, prop.Value);
					count++;
				}
			}



			string output = string.Format(@"UPDATE {0} SET {1} WHERE {2}", tableName, updateStr, whereStr);
			Console.WriteLine(output);
			return output;
		}

		public EthereumWithdrawnTransactionRepository(string connectionString) : base(connectionString)
		{
		}

		public EthereumWithdrawnTransactionRepository(IDbConnection dbConnection) : base(dbConnection)
		{
		}

		public ReturnObject Delete(string Id)
		{
			throw new NotImplementedException();
		}

		public EthereumWithdrawTransaction FindById(string Id)
		{
			throw new NotImplementedException();
		}

		public List<EthereumWithdrawTransaction> FindBySql(string sqlString)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
				var result = Connection.Query<EthereumWithdrawTransaction>(sqlString).ToList();


				return result;
			}
			catch (Exception e)
			{
				return null;
			}
		}

		public ReturnObject Insert(EthereumWithdrawTransaction objectInsert)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
				var result = Connection.InsertTask<string, EthereumWithdrawTransaction>(objectInsert);
				var status = !String.IsNullOrEmpty(result) ? Status.StatusSuccess : Status.StatusError;
				return new ReturnObject
				{
					Status = status,
					Message = status == Status.StatusError ? "Cannot insert" : "Insert Success"
				};
			}
			catch (Exception e)
			{
				return new ReturnObject
				{
					Status = Status.StatusError,
					Message = e.Message
				};
			}
		}

		public ReturnObject Update(EthereumWithdrawTransaction objectUpdate)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
				var result = Connection.Update<EthereumWithdrawTransaction>(objectUpdate);
				var status = result > 0 ? Status.StatusSuccess : Status.StatusError;
				return new ReturnObject
				{
					Status = status,
					Message = status == Status.StatusError ? "Cannot update" : "Update Success"
				};
			}
			catch (Exception e)
			{
				return new ReturnObject
				{
					Status = Status.StatusError,
					Message = e.Message
				};
			}
		}

		public ReturnObject ExcuteSQL(string sqlString, object transaction = null)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();


				var result = 0;
				if (transaction != null)
				{
					MySqlTransaction _transaction = (MySqlTransaction)transaction;
					result = Connection.Execute(sqlString, null, _transaction);
				}
				else
				{
					result = Connection.Execute(sqlString);
				}

				var status = result > 0 ? Status.StatusSuccess : Status.StatusError;
				Console.WriteLine("Excute thing " + result);
				return new ReturnObject
				{
					Status = status,
					Message = status == Status.StatusError ? "Cannot Excute" : "Excute Success"
				};
			}
			catch (Exception e)
			{
				return new ReturnObject
				{
					Status = Status.StatusError,
					Message = e.Message
				};
			}
		}

		public IBlockchainTransaction FindTransactionPending()
		{
	        try
	        {
		        return FindTransactionByStatus(Status.StatusPending);
	        }
	        catch (Exception e)
	        {
		        throw e;
	        }
		}

		public IBlockchainTransaction FindTransactionError()
		{
	        try
	        {
		        return FindTransactionByStatus(Status.StatusError);
	        }
	        catch (Exception e)
	        {
		        throw e;
	        }
		}

		public IBlockchainTransaction FindTransactionByStatus(string status)
		{
	        try
	        {
				if(Connection.State != ConnectionState.Open)
					Connection.Open();

		        var sqlString = $"Select * from {tableName} where Status = @status and InProcess = 0";
		        var result = Connection.QueryFirstOrDefault<EthereumWithdrawTransaction>(sqlString, new {status = status});
		        return result;
	        }
	        catch (Exception e)
	        {
		        throw e;
	        }
		}

		public async Task<ReturnObject> LockForProcess(IBlockchainTransaction transaction)
		{
	        try
	        {
		        if (Connection.State != ConnectionState.Open)
			        Connection.Open();
		        string SqlCommand = "Update " + tableName + " Set Version = Version + 1, InProcess = 1 Where Id = @Id and Version = @Version and InProcess = 0";

		        var update = Connection.Execute(SqlCommand, new {Id = transaction.Id, Version = transaction.Version});
		        if (update == 1)
		        {
			        return new ReturnObject
			        {
				        Status = Status.StatusSuccess,
				        Message = "Update Success",
			        };
		        }
		        return new ReturnObject
		        {
			        Status = Status.StatusError,
			        Message = "Update Fail",
		        };
	        }
	        catch (Exception e)
	        {
		        throw e;
	        }
		}

		public async Task<ReturnObject> ReleaseLock(IBlockchainTransaction transaction)
		{
	        try
	        {
		        if (Connection.State != ConnectionState.Open)
			        Connection.Open();
		        string SqlCommand = "Update " + tableName + " Set Version = Version + 1, InProcess = 0 Where Id = @Id and Version = @Version and InProcess = 1";

		        var update = Connection.Execute(SqlCommand, new {Id = transaction.Id, Version = transaction.Version});
		        if (update == 1)
		        {
			        return new ReturnObject
			        {
				        Status = Status.StatusSuccess,
				        Message = "Update Success",
			        };
		        }
		        return new ReturnObject
		        {
			        Status = Status.StatusError,
			        Message = "Update Fail",
		        };
	        }
	        catch (Exception e)
	        {
		        throw e;
	        }
		}

		public async Task<ReturnObject> SafeUpdate(IBlockchainTransaction transaction)
		{
	        try
	        {
		        if (Connection.State != ConnectionState.Open)
			        Connection.Open();
		        string SqlCommand = "Update " + tableName + " Set Version = Version + 1, InProcess = 0, Status = @Status, UpdatedAt = @UpdatedAt Where Id = @Id and Version = @Version and InProcess = 1";

		        var update = Connection.Execute(SqlCommand, new {Id = transaction.Id, Version = transaction.Version, Status = transaction.Status, UpdatedAt = transaction.UpdatedAt});
		        if (update == 1)
		        {
			        return new ReturnObject
			        {
				        Status = Status.StatusSuccess,
				        Message = "Update Success",
			        };
		        }
		        return new ReturnObject
		        {
			        Status = Status.StatusError,
			        Message = "Update Fail",
		        };
	        }
	        catch (Exception e)
	        {
		        throw e;
	        }
		}

		public object GetTransaction()
		{
			if (Connection.State != ConnectionState.Open)
				Connection.Open();
			return Connection.BeginTransaction();
		}

		public void TransactionCommit(object transaction)
		{
			MySqlTransaction _transaction = (MySqlTransaction)transaction;
			_transaction.Commit();
		}

		public void TransactionRollback(object transaction)
		{
			MySqlTransaction _transaction = (MySqlTransaction)transaction;
			_transaction.Rollback();
		}
	}
}
