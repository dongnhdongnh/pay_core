using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public static class JsonHelper
{
    public static T DeserializeObject<T>(string json, JsonSerializerSettings setting = null)
    {
        return JsonConvert.DeserializeObject<T>(json, setting);
    }

    public static T FromJson<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, JsonHelper.ConvertSettings);
    }

    public static string ToJson<T>(this T obj)
    {
        return JsonConvert.SerializeObject(obj, JsonHelper.ConvertSettings);
    }


    public static string SerializeObject(Object obj, JsonSerializerSettings setting = null)
    {
        return JsonConvert.SerializeObject(obj, setting);
    }
        
    public static readonly JsonSerializerSettings ConvertSettings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
//        NullValueHandling = NullValueHandling.Ignore,
        Converters = {
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        }
    };
}