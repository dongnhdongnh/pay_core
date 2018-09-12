using System;
using System.Data;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.EthereumBusiness
{
	using BlockchainBusiness;
	using System.Collections.Generic;
	using Vakapay.Models.Entities.ETH;

	public class EthereumBusiness : BlockchainBusiness
	{
		private EthereumRpc ethereumRpc { get; set; }

		public EthereumBusiness(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true) :
			base(_vakapayRepositoryFactory, isNewConnection)
		{
			ethereumRpc = new EthereumRpc("http://127.0.0.1:9900/");
		}
		public ReturnObject SendTransaction(EthereumWithdrawTransaction blockchainTransaction)
		{
			try
			{
				var _rpcResult = ethereumRpc.SendTransactionWithPassphrase(blockchainTransaction.FromAddress, blockchainTransaction.ToAddress, blockchainTransaction.Amount, "password");
				if (_rpcResult.Status == Status.StatusError)
					return _rpcResult;
				EthRPCJson.Getter _getter = new EthRPCJson.Getter(_rpcResult.Data);
				var ethereumwithdrawRepo = vakapayRepositoryFactory.GetEthereumWithdrawnTransactionRepository(DbConnection);
				blockchainTransaction.Id = CommonHelper.GenerateUuid();
				blockchainTransaction.Hash = (String)_getter.result;
				blockchainTransaction.Status = Status.StatusPending;
				blockchainTransaction.CreatedAt = CommonHelper.GetUnixTimestamp().ToString();
				blockchainTransaction.UpdatedAt = CommonHelper.GetUnixTimestamp().ToString();
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

				var ethereumAddressRepo = vakapayRepositoryFactory.GetEthereumAddressRepository(DbConnection);

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

		public void ScanBlock()
		{
			int blockNumber = 0;
			var _result = ethereumRpc.GetBlockNumber();
			if (_result.Status == Status.StatusError)
				return;
			EthRPCJson.Getter _getter = new EthRPCJson.Getter(_result.Data);
			if (!_getter.result.ToString().HexToInt(out blockNumber))
				return;
			Console.WriteLine("block number " + blockNumber);
			for (int i = blockNumber - 2; i <= blockNumber; i++)
			{
				_result = ethereumRpc.FindBlockByNumber(i);
				if (_result.Status == Status.StatusError)
					return;
				_getter = new EthRPCJson.Getter(_result.Data);
				Console.WriteLine(JsonHelper.SerializeObject(_getter.result));
			}

			String _query = String.Format("SELECT * FROM vakapay.ethereumwithdrawtransaction Where Status='{0}'", Status.StatusPending);
			var ethereumwithdrawRepo = vakapayRepositoryFactory.GetEthereumWithdrawnTransactionRepository(DbConnection);
			List<EthereumWithdrawTransaction> _pending = ethereumwithdrawRepo.FindBySql(_query);
			Console.WriteLine(_pending.Count);
		}
	}
}