using System;
using System.Net;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;

namespace Vakapay.BitcoinBusiness
{
    using BlockchainBusiness;
    public class BitcoinBusiness : BlockchainBusiness
    {
        private BitcoinRpc bitcoinRpc { get; set; }
        
        
        public BitcoinBusiness(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true) : 
            base(_vakapayRepositoryFactory, isNewConnection)
        {
            bitcoinRpc = new BitcoinRpc("http://127.0.0.1:18443");
            bitcoinRpc.Credentials = new NetworkCredential("bitcoinrpc", "wqfgewgewi");
        }
        
        // <summary>
        // Returns a new bitcoin address for receiving payments.
        // If [account] is specified (recommended), it is added to the address book so payments 
        // received with the address will be credited to [account].
        // </summary>
        // <param name="a_account"></param>
        // bitcoin ver 16 GetNewAddress(Account); ver 17 GetNewAddress(Label)
        public String CreateNewAddAddress(string Account = "")
        {
            try
            {    
                var address = bitcoinRpc.GetNewAddress(Account);
                return address;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            
        }
        
        
    }
    
    
}