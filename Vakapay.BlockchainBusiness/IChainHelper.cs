using System.Collections.Generic;

namespace Vakapay.BlockchainBusiness
{
    public interface IChainHelper
    {
        IBlockchainRPC RpcClient { get; set; }
        /// <summary>
        /// Get list of streaming block with transactions from specify block to last trusted block
        /// </summary>
        /// <param name="startBlock">startBlock Specify block to start, if 0 then is last trusted block</param>
        /// <returns>Enumerator of streaming block with transactions</returns>
        IEnumerable<object> StreamBlock(uint startBlock=0);


        /// <summary>
        /// Parse transaction from block info
        /// </summary>
        /// <param name="block"></param>
        void ParseTransaction(object block);
    }
}