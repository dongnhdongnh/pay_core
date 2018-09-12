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
}