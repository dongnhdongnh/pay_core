using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
	public class EthereumAddressRepository : MySqlBaseRepository<EthereumAddress>, IEthereumAddressRepository
	{
		public EthereumAddressRepository(string connectionString) : base(connectionString)
		{
		}

		public EthereumAddressRepository(IDbConnection dbConnection) : base(dbConnection)
		{
		}

		public EthereumAddress FindByAddress(string address)
		{
			throw new NotImplementedException();
		}

		//		public ReturnObject Update(EthereumAddress objectUpdate)
		//		{
		//			throw new System.NotImplementedException();
		//		}
		//
		//		public ReturnObject Delete(string Id)
		//		{
		//			throw new System.NotImplementedException();
		//		}
		//
		public ReturnObject Insert1Async(EthereumAddress objectInsert)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
				var result = Connection.InsertTask<string, EthereumAddress>(objectInsert);
				var status = !String.IsNullOrEmpty(result) ? Status.StatusSuccess : Status.StatusError;
				return new ReturnObject
				{
					Status = status,
					Message = status == Status.StatusError ? "Cannot insert" : "Insert Success"
				};
			}
			catch (Exception e)
			{
				Console.WriteLine("FAIL" + e.ToString());
				return new ReturnObject
				{
					Status = Status.StatusError,
					Message = e.ToString()
				};
			}
		}
		//
		//		public EthereumAddress FindById(string Id)
		//		{
		//			throw new System.NotImplementedException();
		//		}
		//
		//		public List<EthereumAddress> FindBySql(string sqlString)
		//		{
		//			throw new System.NotImplementedException();
		//		}

		//	public EthereumAddress FindByAddress(string address)
		//{
		//	throw new NotImplementedException();
		//}

		public Task<ReturnObject> InsertAddress(string address, string walletId, string other)
		{
			EthereumAddress insertObject = new EthereumAddress()
			{
				Id = CommonHelper.GenerateUuid(),
				Address = address,
				WalletId = walletId,
				Password = other,
				CreatedAt = (int)CommonHelper.GetUnixTimestamp(),
				UpdatedAt = (int)CommonHelper.GetUnixTimestamp()
			};

			return Task.Run(() => this.Insert1Async(insertObject));
			//return new Task() { };
			//throw new NotImplementedException();
		}
	}
}
