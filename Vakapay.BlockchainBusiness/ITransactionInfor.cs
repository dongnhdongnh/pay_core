using System;
using System.Collections.Generic;
using System.Text;

namespace Vakapay.BlockchainBusiness
{
	public abstract class ITransactionInfor
	{
		public string from;
		public string to;
		public string value;
		public string hash;
		public string blockHash;
		public int blockNumber;
		public string transactionIndex;
		public string gas;
		public string gasPrice;
		public string input;
		public string nonce;
	}
}
