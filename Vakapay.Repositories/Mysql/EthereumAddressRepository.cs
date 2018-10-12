using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vakapay.Commons.Constants;
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
//		public ReturnObject Insert(EthereumAddress objectInsert)
//		{
//			try
//			{
//				if (Connection.State != ConnectionState.Open)
//					Connection.Open();
//				var result = Connection.InsertTask<string, EthereumAddress>(objectInsert);
//				var status = !String.IsNullOrEmpty(result) ? Status.StatusSuccess : Status.StatusError;
//				return new ReturnObject
//				{
//					Status = status,
//					Message = status == Status.StatusError ? "Cannot insert" : "Insert Success"
//				};
//			}
//			catch (Exception e)
//			{
//				throw e;
//			}
//		}
//
//		public EthereumAddress FindById(string Id)
//		{
//			throw new System.NotImplementedException();
//		}
//
//		public List<EthereumAddress> FindBySql(string sqlString)
//		{
//			try
//			{
//				if (Connection.State != ConnectionState.Open)
//					Connection.Open();
//				var result = Connection.Query<EthereumAddress>(sqlString);
//				return result.ToList();
//			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e);
//				return null;
//			}
//		}

	    public EthereumAddress FindByAddress(string address)
	    {
		    try
		    {
			    string query = $"SELECT * FROM EthereumAddress WHERE Address = '{address}'";
			    List<EthereumAddress> etherAddress = FindBySql(query);
			    if (etherAddress == null || etherAddress.Count == 0)
				    return null;
			    return etherAddress.First();
		    }
		    catch (Exception e)
		    {
			    Console.WriteLine(e);
			    return null;
		    }
	    }
	    
		public Task<ReturnObject> InsertAddress(string address, string walletId, string other)
		{
			EthereumAddress insertObject = new EthereumAddress()
			{
				Id = CommonHelper.GenerateUuid(),
				Address = address,
				WalletId = walletId,
				Password = other,
				CreatedAt = (int)CommonHelper.GetUnixTimestamp(),
				UpdatedAt = (int)CommonHelper.GetUnixTimestamp(),
				Status = Status.STATUS_ACTIVE
			};

			return Task.Run(() => this.Insert(insertObject));
			//return new Task() { };
			//throw new NotImplementedException();
		}
    }
}
