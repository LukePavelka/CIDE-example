//    using CideClient.Structures.PlotterSpansh;
//
//    var plotterSpanshResult = CideClient.Structures.PlotterSpansh.PlotterSpanshResult.FromJson(jsonString);

namespace CideClient.Structures.PlotterSpansh
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using J = Newtonsoft.Json.JsonPropertyAttribute;
    using R = Newtonsoft.Json.Required;
    using N = Newtonsoft.Json.NullValueHandling;

    public partial class PlotterSpanshResult
    {
        [J("job", NullValueHandling = N.Ignore)]    public string Job { get; set; }   
        [J("status", NullValueHandling = N.Ignore)] public string Status { get; set; }
    }

    public partial class PlotterSpanshResult
    {
        public static PlotterSpanshResult FromJson(string json) => JsonConvert.DeserializeObject<PlotterSpanshResult>(json, CideClient.Structures.PlotterSpansh.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this PlotterSpanshResult self) => JsonConvert.SerializeObject(self, CideClient.Structures.PlotterSpansh.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
