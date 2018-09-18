﻿using Dapper;
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
	public class VakacoinDepositTransactionRepository : MysqlBaseConnection, IVakacoinDepositTransactionRepository
	{
		public VakacoinDepositTransactionRepository(string connectionString) : base(connectionString)
		{
		}

		public VakacoinDepositTransactionRepository(IDbConnection dbConnection) : base(dbConnection)
		{
		}

		public ReturnObject Delete(string Id)
		{
			throw new NotImplementedException();
		}

		public VakacoinDepositTransaction FindById(string Id)
		{
			throw new NotImplementedException();
		}

		public List<VakacoinDepositTransaction> FindBySql(string sqlString)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
				var result = Connection.Query<VakacoinDepositTransaction>(sqlString).ToList();

				return result;
			}
			catch (Exception e)
			{
				return null;
			}
		}

		public ReturnObject Insert(VakacoinDepositTransaction objectInsert)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
				var result = Connection.InsertTask<string, VakacoinDepositTransaction>(objectInsert);
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

		public ReturnObject Update(VakacoinDepositTransaction objectUpdate)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
				var result = Connection.Update<VakacoinDepositTransaction>(objectUpdate);
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
			throw new NotImplementedException();
		}

		public IBlockchainTransaction FindTransactionError()
		{
			throw new NotImplementedException();
		}

		public IBlockchainTransaction FindTransactionByStatus(string status)
		{
			throw new NotImplementedException();
		}

		public async Task<ReturnObject> LockForProcess(IBlockchainTransaction transaction)
		{
			throw new NotImplementedException();
		}

		public async Task<ReturnObject> ReleaseLock(IBlockchainTransaction transaction)
		{
			throw new NotImplementedException();
		}

		public async Task<ReturnObject> SafeUpdate(IBlockchainTransaction transaction)
		{
			throw new NotImplementedException();
		}
	}
}
