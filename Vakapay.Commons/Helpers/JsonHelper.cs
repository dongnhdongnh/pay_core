using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class JsonHelper
{
    public static T DeserializeObject<T>(string json)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
    }

    // public static string SerializeObject(Object obj)
    public static string SerializeObject(Object obj)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
    }
        
    public static readonly JsonSerializerSettings ConvertSettings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters = {
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        },
    };
}

namespace Vakapay.Commons.Helpers
{
       // public static string SerializeObject(Object obj)
        public static string SerializeObject(Object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
    }
}