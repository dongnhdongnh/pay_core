using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;

namespace Vakapay.Repositories.Mysql.Base
{
    public class MultiThreadUpdateEntityRepository<TEntity> : MySqlBaseRepository<TEntity>
        where TEntity : MultiThreadUpdateEntity
    {
        public MultiThreadUpdateEntityRepository(string connectionString) : base(connectionString)
        {
        }

        public MultiThreadUpdateEntityRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }


        public TEntity FindRowPending()
        {
            try
            {
                return FindRowByStatus(Status.STATUS_PENDING);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public List<TEntity> FindRowsPending()
        {
            try
            {
                return FindRowsByStatus(Status.STATUS_PENDING);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public List<TEntity> FindRowsInProcess()
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var sqlString = $"Select * from {TableName} where InProcess = 1";
                var result = Connection.Query<TEntity>(sqlString).ToList();
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public TEntity FindRowError()
        {
            try
            {
                return FindRowByStatus(Status.STATUS_ERROR);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public TEntity FindRowByStatus(string status)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var sqlString = $"Select * from {TableName} where Status = @status and InProcess = 0";
                var result =
                    Connection.QueryFirstOrDefault<TEntity>(sqlString, new {status = status});
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        public List<TEntity> FindRowsByStatus(string status)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                //Console.WriteLine("FIND TRANSACTION BY STATUS");
                var sqlString = $"Select * from {TableName} where Status = @status and InProcess = 0";
                var result = Connection.Query<TEntity>(sqlString, new {status = status})
                    .ToList();
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ReturnObject> LockForProcess(TEntity row)
        {
            //Console.WriteLine("LockForProcess");
            int cache = row.Version;

            var setQuery = new Dictionary<string, string>
            {
                {nameof(row.Version), (row.Version + 1).ToString()}, {nameof(row.IsProcessing), "1"}
            };

            var whereValue = new Dictionary<string, string>
            {
                {nameof(row.Id), row.Id},
                {nameof(row.Version), row.Version.ToString()},
                {nameof(row.IsProcessing), "0"}
            };

            if (cache != row.Version)
            {
                Console.WriteLine("fucking error");
            }

            return ExcuteSQL(SqlHelper.Query_Update(TableName, setQuery, whereValue));

            //try
            //{
            //	if (Connection.State != ConnectionState.Open)
            //		Connection.Open();
            //	var sqlCommand = "Update " + TableName +
            //						" Set Version = Version + 1, InProcess = 1 Where Id = @Id and Version = @Version and InProcess = 0";

            //	var update = Connection.Execute(sqlCommand, new { Id = transaction.Id, Version = transaction.Version });
            //	if (update == 1)
            //	{
            //		return new ReturnObject
            //		{
            //			Status = Status.StatusSuccess,
            //			Message = "Update Success",
            //		};
            //	}

            //	return new ReturnObject
            //	{
            //		Status = Status.StatusError,
            //		Message = "Update Fail",
            //	};
            //}
            //catch (Exception e)
            //{
            //	throw;
            //}
        }

        public async Task<ReturnObject> ReleaseLock(TEntity row)
        {
            //Console.WriteLine("ReleaseLock");
            var setQuery = new Dictionary<string, string>
            {
                {nameof(row.Version), (row.Version + 1).ToString()}, {nameof(row.IsProcessing), "0"}
            };
            var whereValue = new Dictionary<string, string>
            {
                {nameof(row.Id), row.Id},
                {nameof(row.Version), row.Version.ToString()},
                {nameof(row.IsProcessing), "1"}
            };

            return ExcuteSQL(SqlHelper.Query_Update(TableName, setQuery, whereValue));


            //try
            //{
            //	if (Connection.State != ConnectionState.Open)
            //		Connection.Open();
            //	var sqlCommand = "Update " + TableName +
            //						" Set Version = Version + 1, InProcess = 0  Where Id = @Id and Version = @Version and InProcess = 1";

            //	var update = Connection.Execute(sqlCommand, new { Id = transaction.Id, Version = transaction.Version });
            //	if (update == 1)
            //	{
            //		return new ReturnObject
            //		{
            //			Status = Status.StatusSuccess,
            //			Message = "Update Success",
            //		};
            //	}

            //	return new ReturnObject
            //	{
            //		Status = Status.StatusError,
            //		Message = "Update Fail",
            //	};
            //}
            //catch (Exception e)
            //{
            //	throw;
            //}
        }

        public async Task<ReturnObject> SafeUpdate(TEntity row, IEnumerable<string> updatePropStrings)
        {
            //Console.WriteLine("SafeUpdate");
            try
            {
                var cache = row.Version;
                var setQuery = new Dictionary<string, string>
                {
                    {nameof(row.Version), (row.Version + 1).ToString()},
                    {nameof(row.IsProcessing), "0"},
                    {nameof(row.Status), row.Status},
                    {nameof(row.UpdatedAt), CommonHelper.GetUnixTimestamp().ToString()}
                };

                foreach (var prop in updatePropStrings)
                {
                    var value = typeof(TEntity).GetProperty(prop).GetValue(row);

                    if (value!=null)
                    {
                        setQuery.Add(prop, value.ToString());
                    }
                }

                var whereValue = new Dictionary<string, string>
                {
                    {nameof(row.Id), row.Id},
                    {nameof(row.Version), row.Version.ToString()},
                    {nameof(row.IsProcessing), "1"}
                };

                if (cache != row.Version)
                {
                    Console.WriteLine("fucking error");
                }

                return ExcuteSQL(SqlHelper.Query_Update(TableName, setQuery, whereValue));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            //try
            //{
            //	if (Connection.State != ConnectionState.Open)
            //		Connection.Open();
            //	var sqlCommand = "Update " + TableName +
            //						" Set Version = Version + 1, InProcess = 0, Status = @Status, UpdatedAt = @UpdatedAt,Hash = @Hash Where Id = @Id and Version = @Version and InProcess = 1";

            //	var update = Connection.Execute(sqlCommand,
            //		new
            //		{
            //			Id = transaction.Id,
            //			Version = transaction.Version,
            //			Status = transaction.Status,
            //			UpdatedAt = transaction.UpdatedAt,
            //			Hash = transaction.Hash
            //		});
            //	if (update == 1)
            //	{
            //		return new ReturnObject
            //		{
            //			Status = Status.StatusSuccess,
            //			Message = "Update Success",
            //		};
            //	}

            //	return new ReturnObject
            //	{
            //		Status = Status.StatusError,
            //		Message = "Update Fail",
            //	};
            //}
            //catch (Exception e)
            //{
            //	throw;
            //}
        }

        public List<TEntity> FindTransactionHistory(int offset = -1, int limit = -1,
            string[] orderBy = null)
        {
            try
            {
                var setQuery = new Dictionary<string, string>
                {
                    {nameof(BlockchainTransaction.Status), Status.STATUS_COMPLETED}
                };

                return FindBySql(SqlHelper.Query_Search(TableName, setQuery, limit, offset, orderBy))
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
                //	throw;
            }

            //	throw new NotImplementedException();
        }
    }
}