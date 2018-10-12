using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

public class JsonHelper
{
    public static T DeserializeObject<T>(string json, JsonSerializerSettings setting = null)
    {
        return JsonHelper.DeserializeObject<T>(json, setting);
    }

    public static string SerializeObject(Object obj, JsonSerializerSettings setting = null)
    {
        return JsonHelper.SerializeObject(obj, setting);
    }
        
    public static readonly JsonSerializerSettings ConvertSettings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters = {
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        }
    };
}