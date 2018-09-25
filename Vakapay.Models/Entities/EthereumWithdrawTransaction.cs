using System;
using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
	[Table("ethereumwithdrawtransaction")]
	public class EthereumWithdrawTransaction : BlockchainTransaction
	{
	}
}
