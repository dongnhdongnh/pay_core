using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities.ETH;
using Vakapay.Commons.Helpers;
using Vakapay.EthereumBussiness;

namespace Vakapay.EthereumBusiness
{
	/// <summary>
	/// This class is communicate with ethereum network throught rpc api
	/// </summary>
	public class EthereumRpc
	{
		public string EndPointUrl { get; set; }

		public EthereumRpc(string endPointUrl)
		{
			EndPointUrl = endPointUrl;
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
				Console.WriteLine("=====================" + RpcName + "=======================");
				// Set a default policy level for the "http:" and "https" schemes.
				HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
				HttpWebRequest.DefaultCachePolicy = policy;
				var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:9900/");
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
					Console.WriteLine(json);
					streamWriter.Write(json);
					streamWriter.Flush();
					streamWriter.Close();
				}
				//Console.WriteLine("FROM CACHE:" + httpWebRequest.GetResponse().IsFromCache);
				var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				Console.WriteLine("IsFromCache? {0}", httpResponse.IsFromCache);
				using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
				{
					var result = streamReader.ReadToEnd();
					//EthRPCJson.Getter _getter = new EthRPCJson.Getter(result);
					Console.WriteLine(result);
					//Console.WriteLine("FROM CACHE:" + httpWebRequest.GetResponse().IsFromCache);
					return new ReturnObject
					{
						Status = Status.StatusCompleted,
						//Message = _getter,
						Data = result,
					};
				}
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
		/// Send Transaction with  Passphrase
		/// </summary>
		/// <param name="From"></param>
		/// <param name="ToAddress"></param>
		/// <param name="amount"></param>
		/// <param name="passphrase"></param>
		/// <returns></returns>
		public ReturnObject SendTransactionWithPassphrase(string From, string ToAddress, decimal amount, string passphrase)
		{
			EthRPCJson.TransactionInfor _sender = new EthRPCJson.TransactionInfor()
			{
				from = From,
				to = ToAddress,
				value = ((int)amount).IntToHex()
			};
			//var tx = { from: "0x391694e7e0b0cce554cb130d723a9d27458f9298", to: "0xafa3f8684e54059998bc3a7b0d2b0da075154d66", value: web3.toWei(1.23, "ether")};
			return EthereumSendRPC(EthereumRPCList.RPCName.personal_sendTransaction, new Object[] { _sender, passphrase });
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
		/// <summary>
		/// This function will be create ethereum address with password
		/// return Return Object with data property is address generated
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public ReturnObject CreateAddress(string password)
		{
			return EthereumSendRPC(EthereumRPCList.RPCName.personal_newAccount, new Object[] { password });
		}

		public ReturnObject FindTransactionByBlockNumberAndIndex(int blockNumber, int transactionIndex)
		{
			return EthereumSendRPC(EthereumRPCList.RPCName.eth_getTransactionByBlockNumberAndIndex, new Object[] { blockNumber.IntToHex(), transactionIndex.IntToHex() });
		}

		public ReturnObject FindTransactionByHash(string hash)
		{
			return EthereumSendRPC(EthereumRPCList.RPCName.eth_getTransactionByHash, new Object[] { hash });
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
			return EthereumSendRPC(EthereumRPCList.RPCName.eth_blockNumber);
		}

	}
}