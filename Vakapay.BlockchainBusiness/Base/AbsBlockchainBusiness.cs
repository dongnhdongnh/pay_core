using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.BlockchainBusiness.Base
{
	public abstract class AbsBlockchainBusiness
	{
		public IVakapayRepositoryFactory VakapayRepositoryFactory { get; }
		public IDbConnection DbConnection { get; }

		public AbsBlockchainBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true)
		{
			VakapayRepositoryFactory = vakapayRepositoryFactory;
			DbConnection = isNewConnection
				? VakapayRepositoryFactory.GetDbConnection()
				: VakapayRepositoryFactory.GetOldConnection();
		}

		/// <summary>
		/// Send Transaction with optimistic lock
		/// Send with multiple thread
		/// </summary>
		/// <param name="repoQuery"></param>
		/// <param name="rpcClass"></param>
		/// <param name="privateKey"></param>
		/// <typeparam name="TBlockchainTransaction"></typeparam>
		/// <returns></returns>
		public async Task<ReturnObject> SendTransactionAsync<TBlockchainTransaction>(
			IRepositoryBlockchainTransaction<TBlockchainTransaction> repoQuery, IBlockchainRPC rpcClass,
			string privateKey = "")
		{
			/*
             * 1. Query Transaction Withdraw pending
             * 2. Update Processing = 1, version = version + 1
             * 3. Commit Transaction
             * 4. Call RPC send transaction
             * 5. Update Transaction Status
             */
			// find transaction pending
			var pendingTransaction = repoQuery.FindTransactionPending();
			if (pendingTransaction?.Id == null)
				return new ReturnObject
				{
					Status = Status.StatusSuccess,
					Message = "Not found Transaction"
				};
			if (DbConnection.State != ConnectionState.Open)
			{
				Console.WriteLine(DbConnection.State);
				DbConnection.Open();
			}


			//begin first transaction
			var transactionScope = DbConnection.BeginTransaction();
			try
			{
				//lock transaction for process
				var resultLock = await repoQuery.LockForProcess(pendingTransaction);
				if (resultLock.Status == Status.StatusError)
				{
					transactionScope.Rollback();
					return new ReturnObject
					{
						Status = Status.StatusSuccess,
						Message = "Cannot Lock For Process"
					};
				}

				//commit transaction
				transactionScope.Commit();
			}
			catch (Exception e)
			{
				transactionScope.Rollback();
				return new ReturnObject
				{
					Status = Status.StatusError,
					Message = e.ToString()
				};
			}

			//Update Version to Model
			pendingTransaction.Version += 1;

			//start send and update

			var transactionDbSend = DbConnection.BeginTransaction();
			try
			{
				//Call RPC Transaction
				//TODO EDIT RPC Class
				var sendTransaction = await rpcClass.SendTransactionAsync(pendingTransaction);
				pendingTransaction.Status = sendTransaction.Status;
				pendingTransaction.InProcess = 0;
				pendingTransaction.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
				pendingTransaction.Hash = sendTransaction.Data;
				var result = await repoQuery.SafeUpdate(pendingTransaction);
				if (result.Status == Status.StatusError)
				{
					transactionDbSend.Rollback();
					return new ReturnObject
					{
						Status = Status.StatusError,
						Message = "Cannot Save Transaction Status"
					};
				}

				transactionDbSend.Commit();
				return new ReturnObject
				{
					Status = sendTransaction.Status,
					Message = sendTransaction.Message
				};
			}
			catch (Exception e)
			{
				//release lock
				var resultRelease = repoQuery.ReleaseLock(pendingTransaction);
				Console.WriteLine(JsonHelper.SerializeObject(resultRelease));
				transactionDbSend.Rollback();
				throw e;
			}
		}


		public async Task<ReturnObject> CreateAddressAsyn<TBlockchainAddress>(
			IRepositoryBlockchainAddress<TBlockchainAddress> repoQuery, IBlockchainRPC rpcClass, string walletId,
			string other)
		{
			try
			{
				var walletRepository = VakapayRepositoryFactory.GetWalletRepository(DbConnection);

				var walletCheck = walletRepository.FindById(walletId);

				if (walletCheck == null)
					return new ReturnObject
					{
						Status = Status.StatusError,
						Message = "Wallet Not Found"
					};

				var results = rpcClass.CreateNewAddress(other);
				if (results.Status == Status.StatusError)
					return results;

				var address = results.Data;


				var resultAddBitcoinAddress = await repoQuery.InsertAddress(walletId, other);
				//
				return resultAddBitcoinAddress;
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

		public async Task<ReturnObject> ScanBlockAsyn(string networkName,
			WalletBusiness.WalletBusiness wallet,
			IRepositoryBlockchainTransaction<BlockchainTransaction> withdrawRepoQuery,
			IBlockchainRPC rpcClass)
		{
			try
			{
				int lastBlock = -1;
				int blockNumber = -1;
				//Get lastBlock from last time
				int.TryParse(CacheHelper.GetCacheString(String.Format(CacheHelper.CacheKey.KEY_SCANBLOCK_LASTSCANBLOCK, networkName)), out lastBlock);
				if (lastBlock < 0)
					lastBlock = 0;
				//get blockNumber:
				var _result = rpcClass.GetBlockNumber();
				if (_result.Status == Status.StatusError)
				{
					throw new Exception("Cant GetBlockNumber");
				}
				if (int.TryParse(_result.Data.ToString(), out blockNumber))
				{
					throw new Exception("Cant parse block number");
				}
				//Search transactions which need to scan:
				var withdrawPendingTransactions = withdrawRepoQuery.FindTransactionsPending();
				if (withdrawPendingTransactions.Count <= 0)
				{
					throw new Exception("withdrawPendingTransactions.Count <= 0");
				}
				//Get list of new block that have transactions
				Console.WriteLine("SCAN FROM " + lastBlock + "___" + blockNumber);
				List<IBlockInfor> blocks = new List<IBlockInfor>();
				for (int i = lastBlock; i <= blockNumber; i++)
				{
					if (i < 0) continue;
					_result = rpcClass.GetBlockHaveTransactionByNumber(i);
					if (_result.Status == Status.StatusError)
					{

						return _result;
					}
					if (_result.Data == null)
						continue;
					IBlockInfor _block = JsonHelper.DeserializeObject<IBlockInfor>(_result.Data.ToString());
					if (_block.transactions.Length > 0)
					{
						blocks.Add(_block);
					}
				}
				CacheHelper.SetCacheString(CacheHelper.CacheKey.KEY_SCANBLOCK_LASTSCANBLOCK, blockNumber.ToString());
				if (blocks.Count <= 0)
				{
					throw new Exception("no blocks have transaction");
				}
				//Get done,List<> blocks now contains all block that have transaction
				//check Transaction and update:
				foreach (IBlockInfor _block in blocks)
				{
					if (withdrawPendingTransactions.Count <= 0)
					{
						//SCAN DONE:
						break;
					}
					for (int i = withdrawPendingTransactions.Count - 1; i >= 0; i--)
					{
						BlockchainTransaction _currentPending = withdrawPendingTransactions[i];
						ITransactionInfor _trans = _block.transactions.SingleOrDefault(x => x.hash.Equals(_currentPending.Hash));
						if (_trans != null)
						{
							Console.WriteLine("HELLO " + _currentPending.Hash);
							_currentPending.BlockNumber = _trans.blockNumber;
							_currentPending.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
							_currentPending.Status = Status.StatusCompleted;
							withdrawRepoQuery.Update(_currentPending);
							withdrawPendingTransactions.RemoveAt(i);
						}

					}
				}

				//check wallet balance and update 
				foreach (IBlockInfor _block in blocks)
				{

					foreach (ITransactionInfor _trans in _block.transactions)
					{
						string _toAddress = _trans.to;
						if (!wallet.CheckExistedAddress(_toAddress))
						{
							//logger.Info(to + " is not exist in Wallet!!!");
							continue;
						}
						else
						{
							//Console.WriteLine("value" + _trans.value);
							int _transaValue = 0;
							if (_trans.value.HexToInt(out _transaValue))
								wallet.UpdateBalance(_toAddress, (Decimal)_transaValue, NetworkName.ETH);
						}
					}
				}


				return new ReturnObject
				{
					Status = Status.StatusCompleted,
					Message = "Scan done"
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