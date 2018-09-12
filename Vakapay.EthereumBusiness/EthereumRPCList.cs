using System;
using System.Collections.Generic;
using System.Text;
using Vakapay.Models.Entities.ETH;
namespace Vakapay.EthereumBussiness
{

	public class EthereumRPCList
	{
		static Dictionary<RPCName, EthRPCJson.Sender> _datas;
		static Dictionary<RPCName, EthRPCJson.Sender> Datas
		{
			get
			{
				if (_datas == null)
				{
					_datas = new Dictionary<RPCName, EthRPCJson.Sender>();
					_datas[RPCName.eth_accounts] = new EthRPCJson.Sender("1", "eth_accounts");
					_datas[RPCName.eth_getBlockByNumber] = new EthRPCJson.Sender("1", "eth_getBlockByNumber");
					_datas[RPCName.eth_getBlockByHash] = new EthRPCJson.Sender("1", "eth_getBlockByHash");
					_datas[RPCName.personal_newAccount] = new EthRPCJson.Sender("74", "personal_newAccount");
					_datas[RPCName.personal_sendTransaction] = new EthRPCJson.Sender("1", "personal_sendTransaction");
					_datas[RPCName.eth_getTransactionByHash] = new EthRPCJson.Sender("1", "eth_getTransactionByHash");
					_datas[RPCName.eth_getTransactionByBlockNumberAndIndex] = new EthRPCJson.Sender("1", "eth_getTransactionByBlockNumberAndIndex");
					_datas[RPCName.eth_sendTransaction] = new EthRPCJson.Sender("1", "eth_sendTransaction");
				}
				return _datas;
			}
		}
		public enum RPCName
		{
			eth_accounts,
			eth_getBlockByNumber,
			eth_getBlockByHash,
			personal_newAccount,
			personal_sendTransaction,
			eth_getTransactionByHash,
			eth_getTransactionByBlockNumberAndIndex,
			eth_sendTransaction

		}
		//private readonly EthRPCJson.Sender Eth_accounts()
		//	{

		//	}

		public static EthRPCJson.Sender GetSender(RPCName rpcName)
		{
			return Datas[rpcName];
		}
	}
}
