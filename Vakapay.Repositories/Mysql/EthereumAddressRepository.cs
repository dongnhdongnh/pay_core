using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class EthereumAddressRepository : MysqlBaseConnection, IEthereumAddressRepository
    {
        public EthereumAddressRepository(string connectionString) : base(connectionString)
        {
        }

		public EthereumAddressRepository(IDbConnection dbConnection) : base(dbConnection)
		{
		}

		public ReturnObject Update(EthereumAddress objectUpdate)
		{
			throw new System.NotImplementedException();
		}

		public ReturnObject Delete(string Id)
		{
			throw new System.NotImplementedException();
		}

		public ReturnObject Insert(EthereumAddress objectInsert)
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
				throw e;
			}
		}

		public EthereumAddress FindById(string Id)
		{
			throw new System.NotImplementedException();
		}

		public List<EthereumAddress> FindBySql(string sqlString)
		{
			throw new System.NotImplementedException();
		}

	    public EthereumAddress FindByAddress(string address)
	    {
		    throw new NotImplementedException();
	    }
    }
}
