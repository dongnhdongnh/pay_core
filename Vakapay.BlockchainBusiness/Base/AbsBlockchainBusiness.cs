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
		public virtual async Task<ReturnObject> SendTransactionAsync<TBlockchainTransaction>(
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
			{
				//if (!CacheHelper.HaveKey("cache"))
				//{
				//	long numberOfTicks = (DateTime.Now.Ticks - startTime);

				//	TimeSpan ts = TimeSpan.FromTicks(numberOfTicks);
				//	double minutesFromTs = ts.TotalMinutes;
				//	CacheHelper.SetCacheString("cache", minutesFromTs.ToString());

				//}
				//Console.WriteLine("END TIME " + CacheHelper.GetCacheString("cache"));
				return new ReturnObject
				{
					Status = Status.StatusSuccess,
					Message = "Not found Transaction"
				};
			}
			if (DbConnection.State != ConnectionState.Open)
			{
				//Console.WriteLine(DbConnection.State);
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


		/// <summary>
		/// Created Address with optimistic lock
		/// </summary>
		/// <param name="repoQuery"></param>
		/// <param name="rpcClass"></param>
		/// <param name="walletId"></param>
		/// <param name="other"></param>
		/// <typeparam name="TBlockchainTransaction"></typeparam>
		/// <returns></returns>
		public virtual async Task<ReturnObject> CreateAddressAsyn<TBlockchainAddress>(
			IAddressRepository<TBlockchainAddress> repoQuery, IBlockchainRPC rpcClass, string walletId,
			string other = "") where TBlockchainAddress : BlockchainAddress
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

				var resultsRPC = rpcClass.CreateNewAddress(other);
				if (resultsRPC.Status == Status.StatusError)
					return resultsRPC;

				var address = resultsRPC.Data;

				//	TBlockchainAddress _newAddress = new TBlockchainAddress();

				var resultDB = await repoQuery.InsertAddress(address, walletId, other);
				
				//
				//if (result.Status == Status.StatusError)
				//{
				//	return new ReturnObject
				//	{
				//		Status = Status.StatusSuccess,
				//		Message = "Cannot Insert Address"
				//	};
				//}

				//update address into wallet db
				var walletBusiness =
					new WalletBusiness.WalletBusiness(VakapayRepositoryFactory);
				var updateWallet =
					walletBusiness.UpdateAddressForWallet(walletId, address);
				if (updateWallet.Status == Status.StatusError)
				{
					return new ReturnObject
					{
						Status = Status.StatusError,
						Message = "Update address fail to WalletDB"
					};
				}

				//get all address = null with same networkName of walletId
				//var wallets =
				//	walletRepository.FindByAddressAndNetworkName(null,
				//		walletCheck.NetworkName);
				//if (wallets == null || wallets.Count <= 0)
				//{
				//	return new ReturnObject
				//	{
				//		Status = Status.StatusCompleted,
				//		Message = "Finish Update"
				//	};
				//}

				//var pass = CommonHelper.RandomString(15);
				//await CreateAddressAsyn<TBlockchainAddress>(repoQuery, rpcClass,
				//	wallets[0].Id, pass);


				return new ReturnObject
				{
					Status = resultDB.Status,
					Message = resultDB.Message
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

		public virtual async Task<ReturnObject> ScanBlockAsyn<TWithDraw, TDeposit, TBlockInfor, TTransactionInfor>(string networkName,
				 WalletBusiness.WalletBusiness wallet,
				 IRepositoryBlockchainTransaction<TWithDraw> withdrawRepoQuery,
				 IRepositoryBlockchainTransaction<TDeposit> depositRepoQuery,
				 IBlockchainRPC rpcClass)
			where TWithDraw : BlockchainTransaction
			where TDeposit : BlockchainTransaction
			where TTransactionInfor : ITransactionInfor
			where TBlockInfor : IBlockInfor<TTransactionInfor>
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
				if (!int.TryParse(_result.Data.ToString(), out blockNumber))
				{
					throw new Exception("Cant parse block number");
				}
				//Get list of new block that have transactions
				if (lastBlock >= blockNumber)
					lastBlock = blockNumber;
				Console.WriteLine("SCAN FROM " + lastBlock + "___" + blockNumber);
				List<TBlockInfor> blocks = new List<TBlockInfor>();
				for (int i = lastBlock; i <= blockNumber; i++)
				{
					if (i < 0) continue;
					_result = rpcClass.GetBlockByNumber(i);
					if (_result.Status == Status.StatusError)
					{

						return _result;
					}
					if (_result.Data == null)
						continue;
					TBlockInfor _block = JsonHelper.DeserializeObject<TBlockInfor>(_result.Data.ToString());
					if (_block.transactions.Length > 0)
					{
						blocks.Add(_block);
					}
				}
				CacheHelper.SetCacheString(String.Format(CacheHelper.CacheKey.KEY_SCANBLOCK_LASTSCANBLOCK, networkName), blockNumber.ToString());
				if (blocks.Count <= 0)
				{
					throw new Exception("no blocks have transaction");
				}
				//Get done,List<> blocks now contains all block that have transaction
				//check Transaction and update:
				//Search transactions which need to scan:

				var withdrawPendingTransactions = withdrawRepoQuery.FindTransactionsNotCompleteOnNet();
				//Scan all block and check Withdraw transaction:
				Console.WriteLine("Scan withdrawPendingTransactions");
				if (withdrawPendingTransactions.Count > 0)
				{
					foreach (TBlockInfor _block in blocks)
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
							int _blockNumber = -1;
							int _fee = 0;

							if (_trans != null)
							{
								_trans.blockNumber.HexToInt(out _blockNumber);
								if (_trans.fee != null)
									_trans.fee.HexToInt(out _fee);
								Console.WriteLine("HELLO " + _currentPending.Hash);
								_currentPending.BlockNumber = _blockNumber;
								_currentPending.Fee = _fee;
								_currentPending.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
								//	_currentPending.Status = Status.StatusCompleted;
								//	_currentPending.InProcess = 0;
								Console.WriteLine("CaLL UPDATE");

								withdrawRepoQuery.Update((TWithDraw)_currentPending);
								withdrawPendingTransactions.RemoveAt(i);
							}

						}
					}
				}
				Console.WriteLine("Scan withdrawPendingTransactions Done");
				//check wallet balance and update 
				foreach (TBlockInfor _block in blocks)
				{

					foreach (ITransactionInfor _trans in _block.transactions)
					{
						string _toAddress = _trans.to;
						string _fromAddress = _trans.from;
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
					Message = e.Message
				};
			}
		}

		public virtual List<BlockchainTransaction> GetHistory<TBlockchainTransaction>(IRepositoryBlockchainTransaction<TBlockchainTransaction> repoQuery)
		{
			try
			{
				return repoQuery.FindTransactionsByStatus(Status.StatusCompleted);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				throw e;
			}

		}
	}
}