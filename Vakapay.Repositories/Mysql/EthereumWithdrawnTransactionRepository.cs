using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
	public class EthereumWithdrawnTransactionRepository : MysqlBaseConnection, IEthereumWithdrawTransactionRepository
	{
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
	}
}
