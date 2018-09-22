using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    [Table("vakacointransaction")]
    public class VakacoinTransaction : BlockchainTransaction
    {
        public string Memo { get; set; }

        public string GetStringAmount()
        {
            return string.Format("{0:0.0000}", Amount) + "VAKA";
        }
    }
}