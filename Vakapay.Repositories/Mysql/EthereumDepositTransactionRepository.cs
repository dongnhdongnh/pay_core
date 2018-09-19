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
	public class EthereumDepositTransactionRepository : MysqlBaseConnection, IEthereumDepositTransactionRepository
	{
		public EthereumDepositTransactionRepository(string connectionString) : base(connectionString)
		{
		}

		public EthereumDepositTransactionRepository(IDbConnection dbConnection) : base(dbConnection)
		{
		}

		public ReturnObject Delete(string Id)
		{
			throw new NotImplementedException();
		}

		public EthereumDepositTransaction FindById(string Id)
		{
			throw new NotImplementedException();
		}

		public List<EthereumDepositTransaction> FindBySql(string sqlString)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
				var result = Connection.Query<EthereumDepositTransaction>(sqlString).ToList();

				return result;
			}
			catch (Exception e)
			{
				return null;
			}
		}

		public ReturnObject Insert(EthereumDepositTransaction objectInsert)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
				var result = Connection.InsertTask<string, EthereumDepositTransaction>(objectInsert);
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

		public ReturnObject Update(EthereumDepositTransaction objectUpdate)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
				var result = Connection.Update<EthereumDepositTransaction>(objectUpdate);
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


		public BlockchainTransaction FindTransactionPending()
		{
			throw new NotImplementedException();
		}

		public List<BlockchainTransaction> FindTransactionsPending()
		{
			throw new NotImplementedException();
		}

		public BlockchainTransaction FindTransactionError()
		{
			throw new NotImplementedException();
		}

		public BlockchainTransaction FindTransactionByStatus(string status)
		{
			throw new NotImplementedException();
		}

		public async Task<ReturnObject> LockForProcess(BlockchainTransaction transaction)
		{
			throw new NotImplementedException();
		}

		public async Task<ReturnObject> ReleaseLock(BlockchainTransaction transaction)
		{
			throw new NotImplementedException();
		}

		public async Task<ReturnObject> SafeUpdate(BlockchainTransaction transaction)
		{
			throw new NotImplementedException();
		}

		public List<BlockchainTransaction> FindTransactionsByStatus(string status)
		{
			throw new NotImplementedException();
		}

		public List<BlockchainTransaction> FindTransactionsInProcess()
		{
			throw new NotImplementedException();
		}
	}
}
