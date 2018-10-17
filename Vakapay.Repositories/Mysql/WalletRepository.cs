using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
	public class WalletRepository : MultiThreadUpdateEntityRepository<Wallet>, IWalletRepository
	{
//		const string TABLENAME = "wallet";
		public WalletRepository(string connectionString) : base(connectionString)
		{
//			this.TableName = TABLENAME;
		}

		public WalletRepository(IDbConnection dbConnection) : base(dbConnection)
		{
//			this.TableName = TABLENAME;
		}

//		public ReturnObject Update(Wallet objectUpdate)
//		{
//			try
//			{
//				if (Connection.State != ConnectionState.Open)
//					Connection.Open();
//				var result = 0;
//				result = Connection.Update<Wallet>(objectUpdate);
//				var status = result > 0 ? Status.StatusSuccess : Status.StatusError;
//				return new ReturnObject
//				{
//					Status = status,
//					Message = status == Status.StatusError ? "Cannot update" : "Update Success"
//				};
//			}
//			catch (Exception e)
//			{
//				return new ReturnObject
//				{
//					Status = Status.StatusError,
//					Message = e.Message
//				};
//			}
//		}
//
//		public ReturnObject Delete(string Id)
//		{
//			throw new System.NotImplementedException();
//		}
//
//		public ReturnObject Insert(Wallet objectInsert)
//		{
//			try
//			{
//				if (Connection.State != ConnectionState.Open)
//					Connection.Open();
//				var result = Connection.InsertTask<string, Wallet>(objectInsert);
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
//		public Wallet FindById(string Id)
//		{
//			try
//			{
//				if (Connection.State != ConnectionState.Open)
//					Connection.Open();
//
//				string sQuery = "SELECT * FROM wallet WHERE Id = @ID";
//				var result = Connection.QuerySingle<Wallet>(sQuery, new { ID = Id });
//
//				return result;
//			}
//			catch (Exception e)
//			{
//				return null;
//			}
//		}
//
//		public List<Wallet> FindBySql(string sqlString)
//		{
//			try
//			{
//				if (Connection.State != ConnectionState.Open)
//					Connection.Open();
//				var result = Connection.Query<Wallet>(sqlString);
//				return result.ToList();
//			}
//			catch (Exception e)
//			{
//				throw e;
//			}
//		}

		public ReturnObject UpdateBalanceWallet(decimal amount, string id, int version)
		{
			try
			{
				if (Connection.State != ConnectionState.Open)
					Connection.Open();
//				Int32 unixTimestamp = (Int32)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
				var unixTimestamp = (int)CommonHelper.GetUnixTimestamp();
				string sQuery =
					$"UPDATE {TableName} SET Balance = Balance + @AMOUNT, Version = @VERSION + 1, UpdatedAt = @TIMESTAMP WHERE Id = @ID AND Version = @VERSION";

				var result = Connection.Query(sQuery,
					new
					{
						ID = id,
						VERSION = version,
						AMOUNT = amount,
						TIMESTAMP = unixTimestamp
					});

				var status = !String.IsNullOrEmpty(result.ToString()) ? Status.STATUS_SUCCESS : Status.STATUS_ERROR;
				return new ReturnObject
				{
					Status = status,
					Message = status == Status.STATUS_ERROR ? "Cannot update" : "Update Success"
				};
			}
			catch (Exception e)
			{
				throw;
			}
		}

//		public Wallet FindByAddress(string address)
//		{
//			try
//			{
//				string query = $"SELECT * FROM {TableName} WHERE Address = '{address}'";
//				List<Wallet> wallets = FindBySql(query);
//				if (wallets == null || wallets.Count == 0)
//					return null;
//				return wallets[0];
//			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e);
//				return null;
//			}
//		}

		public Wallet FindByAddressAndNetworkName(string address, string networkName)
		{
			try
			{
				var walletId = "";
				BlockchainAddress blockchainAddress = null;
				switch (networkName)
				{
					case CryptoCurrency.BTC:
						blockchainAddress = new BitcoinAddressRepository(Connection).FindByAddress(address);
						break;

					case CryptoCurrency.ETH:
						blockchainAddress = new EthereumAddressRepository(Connection).FindByAddress(address);
						break;

					case CryptoCurrency.VAKA:
						blockchainAddress = new VakacoinAccountRepository(Connection).FindByAddress(address);
						break;
				}

				if (blockchainAddress == null)
				{
					return null;
				}

				walletId = blockchainAddress.WalletId;

				return FindById(walletId);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public List<string> GetStringAddresses(string walletId, string networkName)
		{
			try
			{
				List<BlockchainAddress> blockchainAddresses = null;
				switch (networkName)
				{
					case CryptoCurrency.BTC:
						blockchainAddresses = new BitcoinAddressRepository(Connection).FindByWalletId(walletId)
							.ToList<BlockchainAddress>();
						break;

					case CryptoCurrency.ETH:
						blockchainAddresses = new EthereumAddressRepository(Connection).FindByWalletId(walletId)
							.ToList<BlockchainAddress>();
						break;

					case CryptoCurrency.VAKA:
						blockchainAddresses = new VakacoinAccountRepository(Connection).FindByWalletId(walletId)
							.ToList<BlockchainAddress>();
						break;

					default:
						throw new Exception("Network name is not defined!");
				}

				if (blockchainAddresses == null)
				{
					return null;
				}

				var result = new List<string>();

				foreach (var row in blockchainAddresses)
				{
					result.Add(row.GetAddress());
				}

				return result;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public List<BlockchainAddress> GetAddresses(string walletId, string networkName)
		{
			try
			{
				List<BlockchainAddress> blockchainAddresses = null;
				switch (networkName)
				{
					case CryptoCurrency.BTC:
						blockchainAddresses = new BitcoinAddressRepository(Connection).FindByWalletId(walletId)
							.ToList<BlockchainAddress>();
						break;

					case CryptoCurrency.ETH:
						blockchainAddresses = new EthereumAddressRepository(Connection).FindByWalletId(walletId)
							.ToList<BlockchainAddress>();
						break;

					case CryptoCurrency.VAKA:
						blockchainAddresses = new VakacoinAccountRepository(Connection).FindByWalletId(walletId)
							.ToList<BlockchainAddress>();
						break;

					default:
						throw new Exception("Network name is not defined!");
				}

				return blockchainAddresses;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public List<BlockchainAddress> GetAddressesByUserId(string userId)
		{

			try
			{
				var wallets = FindAllWalletByUserId(userId);

				var blockchainAddresses = new List<BlockchainAddress>();

				foreach (var wallet in wallets)
				{
					switch (wallet.Currency)
					{
						case CryptoCurrency.BTC:
							blockchainAddresses.AddRange(new BitcoinAddressRepository(Connection)
								.FindByWalletId(wallet.Id)
								.ToList<BlockchainAddress>());
							break;

						case CryptoCurrency.ETH:
							blockchainAddresses.AddRange(new EthereumAddressRepository(Connection)
								.FindByWalletId(wallet.Id)
								.ToList<BlockchainAddress>());
							break;

						case CryptoCurrency.VAKA:
							blockchainAddresses.AddRange(new VakacoinAccountRepository(Connection)
								.FindByWalletId(wallet.Id)
								.ToList<BlockchainAddress>());
							break;

						default:
							throw new Exception("Network name is not defined!");
					}
				}

				return blockchainAddresses;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public Wallet FindByUserAndNetwork(string userId, string networkName)
		{
			try
			{
				string query = $"SELECT * FROM {TableName} WHERE UserId = '{userId}' AND Currency = '{networkName}'";
				List<Wallet> wallets = FindBySql(query);
				if (wallets == null || wallets.Count == 0)
					return null;
				return wallets.SingleOrDefault();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
			}
		}


//		/// <summary>
//		/// Search wallet by address and nwName
//		/// </summary>
//		/// <param name="address">Null for null address</param>
//		/// <param name="networkName">Null for all networkName</param>
//		/// <returns></returns>
//		public List<Wallet> FindByAddressAndNetworkName(string address, string networkName)
//		{
//			try
//			{
//				string query;
//				if (networkName != null)
//				{
//					if (address == null)
//					{
//						query = $"SELECT * FROM {TableName} WHERE ISNULL(Address) AND NetworkName = '{networkName}'";
//					}
//					else
//					{
//						query = $"SELECT * FROM {TableName} WHERE Address = '{address}' AND NetworkName = '{networkName}'";
//					}
//				}
//				else
//				{
//					query = $"SELECT * FROM {TableName} WHERE Address = '{address}'";
//				}
//				List<Wallet> wallets = FindBySql(query);
//				if (wallets == null || wallets.Count <= 0)
//					return null;
//				return wallets;
//			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e);
//				return null;
//			}
//		}


		public List<Wallet> FindNullAddress()
		{
			try
			{
				string query = $"SELECT * FROM {TableName} WHERE HasAddress='false'";
				List<Wallet> wallets = FindBySql(query);
				if (wallets == null || wallets.Count <= 0)
					return null;
				return wallets;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
			}
		}

		public List<Wallet> FindAllWalletByUser(User user)
		{
			try
			{
				string query = $"SELECT * FROM {TableName} WHERE UserId = '{user.Id}'";
				List<Wallet> wallets = FindBySql(query);
				if (wallets == null || wallets.Count <= 0)
					return null;
				return wallets;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
			}
		}

		public List<Wallet> FindAllWalletByUserId(string id)
		{
			try
			{
				var query = $"SELECT * FROM {TableName} WHERE UserId = '{id}'";
				return FindBySql(query);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
			}
		}

		public Task<ReturnObject> SafeUpdate(Wallet row)
		{
			return base.SafeUpdate(row, new[] {nameof(row.AddressCount)});
		}
	}
}