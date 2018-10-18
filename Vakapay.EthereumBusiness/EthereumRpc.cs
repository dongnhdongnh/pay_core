using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Threading.Tasks;
using Vakapay.BlockchainBusiness;
using Vakapay.Commons.Constants;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Commons.Helpers;
using Vakapay.EthereumBussiness;

namespace Vakapay.EthereumBusiness
{
	/// <summary>
	/// This class is communicate with ethereum network throught RPCClient api
	/// </summary>
	public class EthereumRpc : IBlockchainRPC
	{

		public string EndPointURL { get; set; }
		private static string rootAddress = "0x12890d2cce102216644c59dae5baed380d84830c";
		private static string rootPassword = "password";


		public EthereumRpc(string url)
		{
			EndPointURL = url;
		}

		/// <summary>
		/// Send RPC to get JSON DAta
		/// </summary>
		/// <param name="RpcName">name of RPC method</param>
		/// <param name="ps">params send to RPC</param>
		/// <returns></returns>
		public ReturnObject EthereumSendRPC(EthereumRPCList.RPCName RpcName, Object[] ps = null)
		{
			try
			{
				//Console.WriteLine("=====================" + RpcName + "=======================");
				// Set a default policy level for the "http:" and "https" schemes.
				HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
				HttpWebRequest.DefaultCachePolicy = policy;
				var httpWebRequest = (HttpWebRequest)WebRequest.Create(EndPointURL);
				// Define a cache policy for this request only. 
				HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
				httpWebRequest.CachePolicy = noCachePolicy;
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Method = "POST";
				//EthRPCJson.Sender _sender = new EthRPCJson.Sender()
				//{
				//	id = "1",
				//	jsonrpc = "2.0",
				//	method = "eth_accounts"

				//};
				EthRPCJson.Sender _sender = EthereumRPCList.GetSender(RpcName);
				if (ps != null)
					_sender.param = ps;
				using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
				{
					string json = _sender.GetJSon();
					//Console.WriteLine(json);
					streamWriter.Write(json);
					streamWriter.Flush();
					streamWriter.Close();
				}
				//Console.WriteLine("FROM CACHE:" + httpWebRequest.GetResponse().IsFromCache);
				var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				//if (httpResponse == null)
				//	Console.WriteLine("No response from ETH node");
				//Console.WriteLine("IsFromCache? {0}", httpResponse.IsFromCache);
				using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
				{
					var result = streamReader.ReadToEnd();
					//EthRPCJson.Getter _getter = new EthRPCJson.Getter(result);
					//Console.WriteLine(result);
					//Console.WriteLine("FROM CACHE:" + httpWebRequest.GetResponse().IsFromCache);
					return new ReturnObject
					{
						Status = Status.STATUS_COMPLETED,
						//Message = _getter,
						Data = result,
					};
				}
			}
			catch (Exception e)
			{


				return new ReturnObject
				{
					Status = Status.STATUS_ERROR,
					Message = e.Message
				};
			}

		}
		/// <summary>
		/// Send Transaction with  Passphrase
		/// </summary>
		/// <param name="From"></param>
		/// <param name="ToAddress"></param>
		/// <param name="amount"></param>
		/// <param name="passphrase"></param>
		/// <returns></returns>
		public ReturnObject SendTransactionWithPassphrase(string From, string ToAddress, decimal amount, string passphrase)
		{
			try
			{

				EthRPCJson.TransactionInfor _sender = new EthRPCJson.TransactionInfor()
				{
					from = From,
					to = ToAddress,
					value = ((int)amount).IntToHex()
				};

				//var tx = { from: "0x391694e7e0b0cce554cb130d723a9d27458f9298", to: "0xafa3f8684e54059998bc3a7b0d2b0da075154d66", value: web3.toWei(1.23, "ether")};
				var _result = EthereumSendRPC(EthereumRPCList.RPCName.personal_sendTransaction, new Object[] { _sender, passphrase });
				if (_result.Status == Status.STATUS_ERROR)
				{

					return _result;
				}
				EthRPCJson.Getter _getter = JsonHelper.DeserializeObject<EthRPCJson.Getter>(_result.Data.ToString());
				return new ReturnObject
				{
					Status = Status.STATUS_COMPLETED,
					Data = _getter.result.ToString()
				};

			}
			catch (Exception e)
			{

				return new ReturnObject
				{
					Status = Status.STATUS_ERROR,
					Message = e.Message
				};
			}

		}
		/// <summary>
		/// This function send transaction
		/// </summary>
		/// <param name="From"></param>
		/// <param name="ToAddress"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		public ReturnObject SendTransaction(string From, string ToAddress, decimal amount)
		{
			EthRPCJson.TransactionInfor _sender = new EthRPCJson.TransactionInfor()
			{
				from = From,
				to = ToAddress,
				value = ((int)amount).IntToHex()
			};
			return EthereumSendRPC(EthereumRPCList.RPCName.eth_sendTransaction, new Object[] { _sender });
		}


		public ReturnObject FindTransactionByBlockNumberAndIndex(int blockNumber, int transactionIndex)
		{
			return EthereumSendRPC(EthereumRPCList.RPCName.eth_getTransactionByBlockNumberAndIndex, new Object[] { blockNumber.IntToHex(), transactionIndex.IntToHex() });
		}




		public ReturnObject CreateNewAddress(string password)
		{
			try
			{
				ReturnObject _result = EthereumSendRPC(EthereumRPCList.RPCName.personal_newAccount, new Object[] { password });
				Console.WriteLine(_result);
				if (_result.Status == Status.STATUS_ERROR)
				{

					return _result;
				}
				else
				{
					//	Console.WriteLine();
					EthRPCJson.Getter _getter = JsonHelper.DeserializeObject<EthRPCJson.Getter>(_result.Data.ToString());
					return new ReturnObject
					{
						Status = Status.STATUS_COMPLETED,
						Data = _getter.result.ToString()
					};
				}
			}
			catch (Exception e)
			{

				return new ReturnObject
				{
					Status = Status.STATUS_ERROR,
					Message = e.Message
				};
			}

		}

		public ReturnObject CreateNewAddress()
		{
			throw new NotImplementedException();
		}

		public ReturnObject CreateNewAddress(string privateKey, string publicKey)
		{
			throw new NotImplementedException();
		}

		public ReturnObject SendRawTransaction(string data)
		{
			throw new NotImplementedException();
		}

		public ReturnObject GetBalance(string address)
		{
			throw new NotImplementedException();
		}

		public ReturnObject SignTransaction(string privateKey, object[] transactionData)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Send Transaction Async
		/// </summary>
		/// <param name="blockchainTransaction"></param>
		/// <returns></returns>
		public async Task<ReturnObject> SendTransactionAsync(BlockchainTransaction blockchainTransaction)
		{
			try
			{
				var resultSend = SendTransactionWithPassphrase(rootAddress, blockchainTransaction.ToAddress,
					blockchainTransaction.Amount, rootPassword);
				return resultSend;
			}
			catch (Exception e)
			{
				return new ReturnObject
				{
					Status = Status.STATUS_ERROR,
					Message = e.Message
				};
			}
		}

		public ReturnObject GetBlockByNumber(int blockNumber)
		{
			try
			{
				ReturnObject _result = EthereumSendRPC(EthereumRPCList.RPCName.eth_getBlockByNumber, new Object[] { blockNumber.IntToHex(), true });
				//Console.WriteLine(_result);
				if (_result.Status == Status.STATUS_ERROR)
				{

					return _result;
				}
				else
				{
					//	Console.WriteLine();
					EthRPCJson.Getter _getter = JsonHelper.DeserializeObject<EthRPCJson.Getter>(_result.Data.ToString());
					return new ReturnObject
					{
						Status = Status.STATUS_COMPLETED,
						Data = JsonHelper.SerializeObject(_getter.result)
					};
				}
			}
			catch (Exception e)
			{

				return new ReturnObject
				{
					Status = Status.STATUS_ERROR,
					Message = e.Message
				};
			}


		}

		public async Task<ReturnObject> GetBlockByNumberAsyn(int blockNumber)
		{
			throw new NotImplementedException();
		}



		public ReturnObject FindTransactionByHash(string hash)
		{
			return EthereumSendRPC(EthereumRPCList.RPCName.eth_getTransactionByHash, new Object[] { hash });
		}

		public async Task<ReturnObject> FindTransactionByHashAsyn(string transactionHash)
		{
			throw new NotImplementedException();
		}

		public ReturnObject FindBlockByHash(string hash)
		{
			return EthereumSendRPC(EthereumRPCList.RPCName.eth_getBlockByHash, new Object[] { hash, true });
		}

		public ReturnObject FindBlockByNumber(int number)
		{
			return EthereumSendRPC(EthereumRPCList.RPCName.eth_getBlockByNumber, new Object[] { number.IntToHex(), true });
			//return null;
		}

		public ReturnObject GetAccounts()
		{
			return EthereumSendRPC(EthereumRPCList.RPCName.eth_accounts);
		}
		public ReturnObject GetBlockNumber()
		{

			try
			{
				var _result = EthereumSendRPC(EthereumRPCList.RPCName.eth_blockNumber);
				if (_result.Status == Status.STATUS_ERROR)
				{

					return _result;
				}
				else
				{
					EthRPCJson.Getter _getter = JsonHelper.DeserializeObject<EthRPCJson.Getter>(_result.Data.ToString());
					int _blockNumber = -1;
					if (!_getter.result.ToString().HexToInt(out _blockNumber))
					{
						throw new Exception("cant get int from hex");
					}
					return new ReturnObject
					{
						Status = Status.STATUS_COMPLETED,
						Data = _blockNumber.ToString()
					};
				}
			}
			catch (Exception e)
			{

				return new ReturnObject
				{
					Status = Status.STATUS_ERROR,
					Message = e.Message
				};
			}
			//return 
		}

	}
}