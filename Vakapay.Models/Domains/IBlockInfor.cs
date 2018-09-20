using System;
using System.Collections.Generic;
using System.Text;

namespace Vakapay.Models.Domains
{
	public abstract class IBlockInfor<TTransactionInfor> where TTransactionInfor : ITransactionInfor
	{

		public string number;
		public string hash;
		public TTransactionInfor[] transactions;
		public string transactionsRoot;
		public string totalDifficulty;
	}
}
