using System;
using System.Collections.Generic;
using System.Text;

namespace Vakapay.BlockchainBusiness
{
	public abstract class IBlockInfor
	{

		public string number;
		public string hash;
		public ITransactionInfor[] transactions;
		public string transactionsRoot;
		public string totalDifficulty;
	}
}
