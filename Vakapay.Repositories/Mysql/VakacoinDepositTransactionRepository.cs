﻿using System.Data;
using Vakapay.Models.Entities.VAKA;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class VakacoinDepositTransactionRepository : BlockchainTransactionRepository<VakacoinDepositTransaction>,
        IVakacoinDepositTransactionRepository
    {
        public VakacoinDepositTransactionRepository(string connectionString) : base(connectionString)
        {
        }

        public VakacoinDepositTransactionRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

//		public ReturnObject Delete(string Id)
//		{
//			throw new NotImplementedException();
//		}
//
//		public VakacoinDepositTransaction FindById(string Id)
//		{
//			throw new NotImplementedException();
//		}
//
//		public List<VakacoinDepositTransaction> FindBySql(string sqlString)
//		{
//			try
//			{
//				if (Connection.State != ConnectionState.Open)
//					Connection.Open();
//				var result = Connection.Query<VakacoinDepositTransaction>(sqlString).ToList();
//
//				return result;
//			}
//			catch (Exception e)
//			{
//				return null;
//			}
//		}
//
//		public ReturnObject Insert(VakacoinDepositTransaction objectInsert)
//		{
//			try
//			{
//				if (Connection.State != ConnectionState.Open)
//					Connection.Open();
//				var result = Connection.InsertTask<string, VakacoinDepositTransaction>(objectInsert);
//				var status = !String.IsNullOrEmpty(result) ? Status.StatusSuccess : Status.StatusError;
//				return new ReturnObject
//				{
//					Status = status,
//					Message = status == Status.StatusError ? "Cannot insert" : "Insert Success"
//				};
//			}
//			catch (Exception e)
//			{
//				return new ReturnObject
//				{
//					Status = Status.StatusError,
//					Message = e.Message
//				};
//			}
//		}
//
//		public ReturnObject Update(VakacoinDepositTransaction objectUpdate)
//		{
//			try
//			{
//				if (Connection.State != ConnectionState.Open)
//					Connection.Open();
//				var result = Connection.Update<VakacoinDepositTransaction>(objectUpdate);
//				var status = result > 0 ? Status.StatusSuccess : Status.StatusError;
//				return new ReturnObject
//				{
//					Status = status,
//					Message = status == Status.StatusError ? "Cannot update" : "Update Success"
//				};
//			}
//			catch (Exception e)
//			{
//				return new ReturnObject
//				{
//					Status = Status.StatusError,
//					Message = e.Message
//				};
//			}
//		}

//
//		public BlockchainTransaction FindTransactionPending()
//		{
//			throw new NotImplementedException();
//		}
//
//		public List<BlockchainTransaction> FindTransactionsPending()
//		{
//			throw new NotImplementedException();
//		}
//
//		public BlockchainTransaction FindTransactionError()
//		{
//			throw new NotImplementedException();
//		}
//
//		public BlockchainTransaction FindTransactionByStatus(string status)
//		{
//			throw new NotImplementedException();
//		}
//
//		public async Task<ReturnObject> LockForProcess(BlockchainTransaction transaction)
//		{
//			throw new NotImplementedException();
//		}
//
//		public async Task<ReturnObject> ReleaseLock(BlockchainTransaction transaction)
//		{
//			throw new NotImplementedException();
//		}
//
//		public async Task<ReturnObject> SafeUpdate(BlockchainTransaction transaction)
//		{
//			throw new NotImplementedException();
//		}
//
//		public List<BlockchainTransaction> FindTransactionsByStatus(string status)
//		{
//			throw new NotImplementedException();
//		}
//
//		public List<BlockchainTransaction> FindTransactionsInProcess()
//		{
//			throw new NotImplementedException();
//		}
    }
}