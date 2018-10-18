﻿using Newtonsoft.Json;

namespace Vakapay.Models.Entities.ETH
{
    public class EthereumBlockResponse
    {
        [JsonProperty("number")] public string Number;
        [JsonProperty("hash")] public string Hash;
        [JsonProperty("transactions")] public EthereumTransactionResponse[] TransactionsResponse;
    }

    public class EthereumTransactionResponse
    {
        [JsonProperty("from")] public string From;
        [JsonProperty("to")] public string To;
        [JsonProperty("value")] public string Value;
        [JsonProperty("hash")] public string Hash;
        [JsonProperty("blockHash")] public string BlockHash;
        [JsonProperty("blockNumber")] public string BlockNumber;
        [JsonProperty("fee")] public string Fee;
        [JsonProperty("transactionIndex")] public string TransactionIndex;
        [JsonProperty("gas")] public string Gas;
        [JsonProperty("gasPrice")] public string GasPrice;
        [JsonProperty("input")] public string Input;
        [JsonProperty("nonce")] public string Nonce;
    }
}