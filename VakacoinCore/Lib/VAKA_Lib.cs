using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VakacoinCore.Lib
{

    //This interface must be implemented by any class wishing to be treated as a row of an VAKA table. 
    public interface IVAKATable
    {
        VAKATableMetadata GetMetaData();
    }

    //Defines the properties of a table. Any table implementing IVAKATable will need to return on of these in order to provide details of where table exists on the VAKA chain. 
    public class VAKATableMetadata
    {
        public string primaryKey;
        public string contract;
        public string scope;
        public string table;
        public string key_type = string.Empty;

    }


    public interface IEOAPI
    {
        VAKAAPIMetadata GetMetaData();
    }

    public class VAKAAPIMetadata
    {
        public string uri;
    }

    //////////// Interface used for StringArray /////////////

    public interface IVAKAtringArray
    {
        void SetStringArray(List<String> array);
    }


}

