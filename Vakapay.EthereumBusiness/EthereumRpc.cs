using Vakapay.Models.Domains;

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
        /// This function send transaction
        /// </summary>
        /// <param name="From"></param>
        /// <param name="ToAddress"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public ReturnObject SendTransaction(string From, string ToAddress, decimal amount)
        {
            return null;
        }
        /// <summary>
        /// This function will be create ethereum address with password
        /// return Return Object with data property is address generated
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public ReturnObject CreateAddress(string password)
        {
            return null;
        }

        public ReturnObject FindTransactionByHash(string hash)
        {
            return null;
        }

        public ReturnObject FindBlockByHash(string hash)
        {
            return null;
        }

        public ReturnObject FindBlockByNumber(string number)
        {
            return null;
        }
        
    }
}