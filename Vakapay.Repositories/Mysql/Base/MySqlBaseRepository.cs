using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Repositories.Mysql.Base
{
	public abstract class MySqlBaseRepository<TModel> : MysqlBaseConnection, IRepositoryBase<TModel>
	{
		public MySqlBaseRepository(string connectionString) : base(connectionString, typeof(TModel).Name.ToLower())
		{
		}

		public MySqlBaseRepository(IDbConnection dbConnection) : base(dbConnection, typeof(TModel).Name.ToLower())
		{
		}

		public ReturnObject Update(TModel objectUpdate)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
				var result = Connection.Update(objectUpdate);
				var status = result > 0 ? Status.StatusSuccess : Status.StatusError;
				Logger.Debug(GetClassName() + " =>> update status: " + status);
				return new ReturnObject
				{
					Status = status,
					Message = status == Status.StatusError ? "Cannot Update" : "Update Success",
					Data = ""
				};
			}
			catch (Exception e)
			{
				Logger.Error(GetClassName() + " =>> update fail: " + e.Message);
				return new ReturnObject
				{
					Status = Status.StatusError,
					Message = "Cannot update: " + e.Message,
					Data = ""
				};
			}
		}

		public ReturnObject Delete(string Id)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();

				var result = Connection.Delete(FindById(Id));
				var status = result > 0 ? Status.StatusSuccess : Status.StatusError;
				Logger.Debug(GetClassName() + " =>> Delete status: " + status);
				return new ReturnObject
				{
					Status = status,
					Message = status == Status.StatusError ? "Cannot delete" : "Delete Success"
				};
			}
			catch (Exception e)
			{
				Logger.Error(GetClassName() + " =>> Delete fail: " + e.Message);
				return new ReturnObject
				{
					Status = Status.StatusError,
					Message = "Cannot delete: " + e.Message,
					Data = ""
				};
			}
		}

		public ReturnObject Insert(TModel objectInsert)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
				var result = Connection.InsertTask<string, TModel>(objectInsert);
				var status = !string.IsNullOrEmpty(result) ? Status.StatusSuccess : Status.StatusError;
				Logger.Debug(GetClassName() + " =>> insert status: " + status);
				return new ReturnObject
				{
					Status = status,
					Message = status == Status.StatusError ? "Cannot insert" : "Insert Success"
				};
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Logger.Error(GetClassName() + " =>> insert fail: " + e.Message);
				return new ReturnObject
				{
					Status = Status.StatusError,
					Message = "Cannot insert: " + e.Message,
					Data = ""
				};
			}
		}

		public TModel FindById(string Id)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();

				var sQuery = "SELECT * FROM " + TableName + " WHERE Id = @ID";

				var result = Connection.QuerySingleOrDefault<TModel>(sQuery, new { ID = Id });

				return result;
			}
			catch (Exception e)
			{
				throw;
			}
		}

		public List<TModel> FindBySql(string sqlString)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();

				var result = Connection.Query<TModel>(sqlString);

				return result.ToList();
			}
			catch (Exception e)
			{
				throw;
			}
		}
	}
}