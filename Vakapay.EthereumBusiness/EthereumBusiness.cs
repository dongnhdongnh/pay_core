using System;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.WalletBusiness;
namespace Vakapay.EthereumBusiness
{
	using BlockchainBusiness;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using Vakapay.Models.Entities.ETH;

	public class EthereumBusiness : BlockchainBusiness
	{
		private EthereumRpc ethereumRpc { get; set; }

		public EthereumBusiness(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true, string endPoint = "") :
			base(_vakapayRepositoryFactory, isNewConnection)
		{
			ethereumRpc = new EthereumRpc(endPoint);
		}
		public ReturnObject RunSendTransaction()
		{
			try
			{
				var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
				EthereumWithdrawTransaction _searchValue = new EthereumWithdrawTransaction()
				{
					Status = Status.StatusPending
				};
				Console.WriteLine("OK");
				var searchDic = new Dictionary<string, string>();
				searchDic.Add(nameof(_searchValue.Status), _searchValue.Status);

				List<EthereumWithdrawTransaction> pendings = ethereumwithdrawRepo.FindBySql(ethereumwithdrawRepo.Query_Search(searchDic));
				Console.WriteLine(pendings.Count);
				if (pendings.Count <= 0) throw new Exception("NO PENING");
				else
				{
					foreach (EthereumWithdrawTransaction pending in pendings)
					{
						SendTransaction(pending);
					}
				}
				return new ReturnObject();
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
		public ReturnObject SendTransaction(EthereumWithdrawTransaction blockchainTransaction)
		{
			try
			{
				//UPDATE status to DB:
				var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);

				blockchainTransaction.Status = Status.StatusCompleted;
				blockchainTransaction.InProcess = 1;
				blockchainTransaction.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
				int _currentVersion = blockchainTransaction.Version;
				blockchainTransaction.Version = _currentVersion + 1;
				EthereumWithdrawTransaction blockchainTransactionWhere = new EthereumWithdrawTransaction()
				{
					Id = blockchainTransaction.Id,
					Version = _currentVersion,
					InProcess = 0
				};
				Dictionary<string, string> _updateQuery = new Dictionary<string, string>();
				_updateQuery.Add(nameof(blockchainTransactionWhere.Id), blockchainTransactionWhere.Id);
				_updateQuery.Add(nameof(blockchainTransactionWhere.Version), blockchainTransactionWhere.Version.ToString());
				_updateQuery.Add(nameof(blockchainTransactionWhere.InProcess), blockchainTransactionWhere.InProcess.ToString());
				var _resultExcuteSQL = ethereumwithdrawRepo.ExcuteSQL(ethereumwithdrawRepo.Query_Update(blockchainTransaction, _updateQuery));
				if (_resultExcuteSQL.Status == Status.StatusError)
					return _resultExcuteSQL;

				//if UPDATE DB success ,call RPC to send transaction:
				var _rpcResult = ethereumRpc.SendTransactionWithPassphrase(blockchainTransaction.FromAddress, blockchainTransaction.ToAddress, blockchainTransaction.Amount, "password");
				if (_rpcResult.Status == Status.StatusError)
					return _rpcResult;
				Console.WriteLine("SendTransaction==>rpc result");
				//After call RPC,update transaction Hash to DB 
				EthRPCJson.Getter _getter = new EthRPCJson.Getter(_rpcResult.Data);
				blockchainTransaction.Hash = _getter.result.ToString();
				blockchainTransactionWhere = new EthereumWithdrawTransaction()
				{
					Id = blockchainTransaction.Id,

				};
				_updateQuery = new Dictionary<string, string>();
				_updateQuery.Add(nameof(blockchainTransactionWhere.Id), blockchainTransactionWhere.Id);
				_resultExcuteSQL = ethereumwithdrawRepo.ExcuteSQL(ethereumwithdrawRepo.Query_Update(blockchainTransaction, _updateQuery));

				return _resultExcuteSQL;

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

		public ReturnObject FakePendingTransaction(EthereumWithdrawTransaction blockchainTransaction)
		{
			try
			{
				var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
				blockchainTransaction.Id = CommonHelper.GenerateUuid();
				blockchainTransaction.Status = Status.StatusPending;
				blockchainTransaction.CreatedAt = (int)CommonHelper.GetUnixTimestamp();
				blockchainTransaction.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
				return ethereumwithdrawRepo.Insert(blockchainTransaction);
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

		/// <summary>
		/// call RPC Ethereum to make new address
		/// save address to database
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public ReturnObject CreateNewAddAddress(string walletId)
		{
			try
			{
				string password = CommonHelper.RandomString(15);
				var ResultMakeAddress = ethereumRpc.CreateAddress(password);
				if (ResultMakeAddress.Status == Status.StatusError)
					return ResultMakeAddress;

				EthRPCJson.Getter _getter = new EthRPCJson.Getter(ResultMakeAddress.Data);

				var ethereumAddressRepo = VakapayRepositoryFactory.GetEthereumAddressRepository(DbConnection);

				//TODO Encrypt Password Before save
				var ResultAddEthereumAddress = ethereumAddressRepo.Insert(new EthereumAddress
				{
					Status = Status.StatusActive,
					// Address = ResultMakeAddress.Data,
					Address = (String)_getter.result,
					//Address = (String)_getter.result,
					CreatedAt = (int)CommonHelper.GetUnixTimestamp(),
					Id = CommonHelper.GenerateUuid(),
					Password = password,
					UpdatedAt = (int)CommonHelper.GetUnixTimestamp(),
					WalletId = walletId

				});

				return ResultAddEthereumAddress;
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

		bool isScanning = false;
		public void AutoScanBlock(WalletBusiness.WalletBusiness wallet)
		{
			Thread scanThread = new Thread(() => DoAutoScanBlock(wallet));
			scanThread.Start();


		}
		void DoAutoScanBlock(WalletBusiness.WalletBusiness wallet)
		{
			while (true)

			{

				Console.WriteLine("is scan=" + isScanning);
				if (!isScanning)
					ScanBlock(wallet);
				Thread.Sleep(5000);

			}
		}

		/// <summary>
		/// Scan all blocks and checkBalance or check transaction inprocess
		/// </summary>
		/// <returns></returns>
		public int ScanBlock(WalletBusiness.WalletBusiness wallet)
		{


			Console.WriteLine("START NEW SCAN");
			isScanning = true;
			long _time = CommonHelper.GetUnixTimestamp();
			//Get lastBlock from last time
			int LastBlock = -1;
			int.TryParse(CacheHelper.GetCacheString(CacheHelper.CacheKey.KEY_ETH_LASTSCANBLOCK), out LastBlock);
			if (LastBlock < 0)
				LastBlock = 0;
			//Get number of block
			int blockNumber = 0;
			var _result = ethereumRpc.GetBlockNumber();
			if (_result.Status == Status.StatusError)
			{
				isScanning = false;
				return 0;
			}
			EthRPCJson.Getter _getter = new EthRPCJson.Getter(_result.Data);
			if (!_getter.result.ToString().HexToInt(out blockNumber))
			{
				isScanning = false;
				return 0;
			}
			Console.WriteLine("block number " + blockNumber);

			//Search transactions which need to scan:( inprocess)
			var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
			EthereumWithdrawTransaction _searchValue = new EthereumWithdrawTransaction()
			{
				InProcess = 1
			};
			var searchDic = new Dictionary<string, string>();
			searchDic.Add(nameof(_searchValue.InProcess), _searchValue.InProcess.ToString());
			List<EthereumWithdrawTransaction> _transactionInProcess = ethereumwithdrawRepo.FindBySql(ethereumwithdrawRepo.Query_Search(searchDic));
			if (_transactionInProcess.Count <= 0)
			{
				Console.WriteLine("No _transactionInProcess");
				isScanning = false;
				return 0;
			}

			//Get list of new block that have transactions
			List<EthRPCJson.BlockInfor> blocks = new List<EthRPCJson.BlockInfor>();
			Console.WriteLine("SCAN FROM " + LastBlock + "___" + blockNumber);
			for (int i = LastBlock; i <= blockNumber; i++)
			{
				if (i < 0) continue;
				_result = ethereumRpc.FindBlockByNumber(i);
				if (_result.Status == Status.StatusError)
				{
					isScanning = false;
					return 0;
				}
				if (_result.Data == null)
					continue;
				_getter = new EthRPCJson.Getter(_result.Data);
				EthRPCJson.BlockInfor _block = JsonHelper.DeserializeObject<EthRPCJson.BlockInfor>(_getter.result.ToString());
				if (_block.transactions.Length > 0)
				{
					blocks.Add(_block);
				}
			}
			CacheHelper.SetCacheString(CacheHelper.CacheKey.KEY_ETH_LASTSCANBLOCK, blockNumber.ToString());
			if (blocks.Count <= 0)
			{
				Console.WriteLine("NO BLOCK");
				isScanning = false;
				return 0;
			}
			//Get done,List<> blocks now contains all block that have transaction


			//check Transaction and update:
			var ethereumWithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
			foreach (EthRPCJson.BlockInfor _block in blocks)
			{
				if (_transactionInProcess.Count <= 0)
				{
					//SCAN DONE:
					isScanning = false;
					return 0;
				}
				for (int i = _transactionInProcess.Count - 1; i >= 0; i--)
				{
					EthereumWithdrawTransaction _currentPending = _transactionInProcess[i];
					EthRPCJson.TransactionInfor _trans = _block.transactions.SingleOrDefault(x => x.hash.Equals(_currentPending.Hash));
					if (_trans != null)
					{
						Console.WriteLine("HELLO " + _currentPending.Hash);
						_currentPending.BlockNumber = _trans.blockNumber;
						_currentPending.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
						_currentPending.InProcess = 0;
						ethereumWithdrawRepo.Update(_currentPending);
						_transactionInProcess.RemoveAt(i);
					}

				}
			}
			//check wallet balance and update 
			foreach (EthRPCJson.BlockInfor _block in blocks)
			{

				foreach (EthRPCJson.TransactionInfor _trans in _block.transactions)
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
			//Update Done
			_time = CommonHelper.GetUnixTimestamp() - _time;
			Console.WriteLine(blocks.Count + ",Time=" + _time);
			isScanning = false;
			return blocks.Count;

		}
	}
}
