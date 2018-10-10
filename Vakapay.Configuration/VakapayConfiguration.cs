using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Vakapay.Configuration
{
    public class BitcoinRpcAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
    public static class VakapayConfiguration
    {
        private static List<string> BitcoinNode { get; }
        private static List<string> EthereumNode { get; }
        private static List<string> VakacoinNode { get; }
        private static IConfiguration Configuration { get; }
        
        private static List<List<BitcoinRpcAccount>> BitcoinUsers { get; }
        
        public static string DefaultSqlConnection { get; }

        static VakapayConfiguration()
        {
            Configuration = InitConfiguration();
            
            DefaultSqlConnection = Configuration.GetConnectionString("DefaultConnection");
            
            VakacoinNode = GetListConfiguration(Configuration, "VakacoinNode", "Url");
            BitcoinNode = GetListConfiguration(Configuration, "BitcoinNode", "Url");
            EthereumNode = GetListConfiguration(Configuration, "EthereumNode", "Url");

            BitcoinUsers = GetListBitcoinUser();
        }

        public static string Get(string sectionKey)
        {
            return Configuration[sectionKey];
        }

        public static string GetVakacoinNode()
        {
            return VakacoinNode.Count >= 1 ? VakacoinNode[0] : null;
        }
        
        public static string GetBitcoinNode()
        {
            return BitcoinNode.Count >= 1 ? BitcoinNode[0] : null;
        }

        public static BitcoinRpcAccount GetBitcoinRpcAccount() => BitcoinUsers.Count >= 1
            ? BitcoinUsers[0].Count >= 1 ? BitcoinUsers[0][0] : null
            : null;

        public static string GetEthereumNode()
        {
            return EthereumNode.Count >= 1 ? EthereumNode[0] : null;
        }

        private static List<string> GetListConfiguration(IConfiguration configuration, string section, string key)
        {
            var result = new List<string>();
            var value = "";
            value = configuration[$"{section}:{key}"];
            if (!string.IsNullOrEmpty(value))
            {
                result.Add(value);
            }

            var i = 1;
            while (true)
            {
                value = configuration[$"{section}{i}:{key}"];
                if (string.IsNullOrEmpty(value))
                {
                    break;
                }
                result.Add(value);
                i++;
            }

            return result;
        }
        
        private static List<List<BitcoinRpcAccount>> GetListBitcoinUser()
        {
            var result = new List<List<BitcoinRpcAccount>>();
            const string section = "BitcoinNode";
            const string keyUser = "User";
            const string keyPassword = "Password";

            var i = 1;
            var j = 1;
            while (true)
            {
                if (i > BitcoinNode.Count)
                {
                    break;
                }
                var list = new List<BitcoinRpcAccount>();
                while (true)
                {
                    var rpcAccount = new BitcoinRpcAccount
                    {
                        Username = Configuration[$"{section}{i}:{keyUser}{j}"],
                        Password = Configuration[$"{section}{i}:{keyPassword}{j}"]
                    };

                    if (string.IsNullOrEmpty(rpcAccount.Username))
                    {
                        break;
                    }
                    
                    list.Add(rpcAccount);
                    j++;
                }
                result.Add(list);
                i++;
            }

            return result;
        }
        
        private static IConfiguration InitConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

            if (string.IsNullOrWhiteSpace(environment))
                environment = "Development";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory() /*+ "/../Vakapay.Configuration"*/)
                .AddJsonFile("Vakapay.Backend.Configuration.json", optional: false)
                .AddJsonFile($"Vakapay.Backend.Configuration.{environment}.json", optional: true);
            
            return builder.Build();
        }
    }
}