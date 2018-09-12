using System;
using System.Data;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.EthereumBusiness
{
	using BlockchainBusiness;
	using Vakapay.Models.Entities.ETH;

	public class EthereumBusiness : BlockchainBusiness
	{
		private EthereumRpc ethereumRpc { get; set; }

		public EthereumBusiness(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true) :
			base(_vakapayRepositoryFactory, isNewConnection)
		{
			ethereumRpc = new EthereumRpc("http://endpoint");
		}
		public ReturnObject SendTransaction(EthereumWithdrawTransaction blockchainTransaction)
		{
			return null;
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
				;
			}

		}
	}
}