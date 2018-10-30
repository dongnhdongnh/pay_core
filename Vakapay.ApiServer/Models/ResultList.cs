using System.Collections.Generic;
using Vakapay.Models.Entities;

namespace Vakapay.ApiServer.Models
{
    public class ResultList<T>
    {
        public List<T> List { get; set; }
        public int Total { get; set; }
    }
}