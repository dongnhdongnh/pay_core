using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
		public static string tableName = "ethereumwithdrawtransaction";
		public string Query_Search(string SearchName, string SearchData)
		{
			return String.Format("SELECT * FROM vakapay.ethereumwithdrawtransaction Where {0}='{1}'", SearchName, SearchData);
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


		public IBlockchainTransaction FindTransactionPending()
		{
			return FindTransactionByStatus(Status.StatusPending);
		}

		public IBlockchainTransaction FindTransactionError()
		{
			return FindTransactionByStatus(Status.StatusError);
		}

		public IBlockchainTransaction FindTransactionByStatus(string status)
		{
			try
			{
				if(Connection.State != ConnectionState.Open)
					Connection.Open();
				string sql = $"SELECT * From {tableName} where Status = @status";
				var result = Connection.QuerySingle<EthereumWithdrawTransaction>(sql, new {status});
				return result;
			}
			catch (Exception e)
			{
				return null;
			}
		}

		public async Task<ReturnObject> LockForProcess(IBlockchainTransaction transaction)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
				string SqlCommand = "Update " + tableName + " Set Version = Version + 1, OnProcess = 1 Where Id = @Id and Version = @Version";

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
				return new ReturnObject
				{
					Status = Status.StatusError,
					Message = e.ToString()
				};
			}
		}

		public async Task<ReturnObject> ReleaseLock(IBlockchainTransaction transaction)
		{
			throw new NotImplementedException();
		}

		public async Task<ReturnObject> SafeUpdate(IBlockchainTransaction transaction)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
				string SqlCommand = "Update " + tableName + " Set Version = Version + 1, OnProcess = 0, Status = @Status, UpdatedAt = @UpdatedAt Where Id = @Id and Version = @Version";

				var update = Connection.Execute(SqlCommand, new {Status = transaction.Status, UpdatedAt = transaction.UpdatedAt, Id = transaction.Id, Version = transaction.Version});
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
				return new ReturnObject
				{
					Status = Status.StatusError,
					Message = e.ToString()
				};
			}
		}
	}
}
