namespace Vakapay.Models.Domains
{
	public class Status
	{
		public const string StatusActive = "Active";
		public const string StatusError = "Error";
		public const string StatusCompleted = "Completed";
		public const string StatusSuccess = "Success";
		public const string StatusPending = "Pending";
	}

	public class NetworkName
	{
		public static string[] AllNetwork
		{
			get
			{
				return new string[] { ETH, VAKA, BTC };
			}
		}
		public const string ETH = "Ethereum";
		public const string VAKA = "VAKA";
		public const string BTC = "BTC";
	}
}