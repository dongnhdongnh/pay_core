﻿using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
	[Table("EthereumWithdrawTransaction")]
	public class EthereumWithdrawTransaction : BlockchainTransaction
	{
	}
}
