using System;
using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Entities;

namespace Vakapay.Models.Domains
{
	public abstract class BlockchainAddress
	{
		public string Id { get; set; }
		public string Address { get; set; }
		public string WalletId { get; set; }
		public string Status { get; set; }
		public int CreatedAt { get; set; }
		public int UpdatedAt { get; set; }

		[NotMapped] //database attribute define
		public string Network { get
		{
			switch (this.GetType().Name)
			{
				case nameof(BitcoinAddress):
					return "bitcoin";
				case nameof(EthereumAddress):
					return "ethereum";
				case nameof(VakacoinAccount):
					return "vakacoin";
				default:
					throw new Exception("Network not defined!");
			}
		} }

		public abstract string GetAddress();
		public abstract string GetSecret();
	}
}
