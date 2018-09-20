﻿using System;
using System.Collections.Generic;
using System.Text;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities.ETH
{
	public class ETHEntities
	{
		public class ETHBlockInfor : IBlockInfor<ETHTransaction>
		{

		}

		public class ETHTransaction : ITransactionInfor
		{ }
	}
}