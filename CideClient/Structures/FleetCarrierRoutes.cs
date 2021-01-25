// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using CideClient.Structures;
//
//    var fleetCarrierRoutes = FleetCarrierRoutes.FromJson(jsonString);

namespace CideClient.Structures
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using J = Newtonsoft.Json.JsonPropertyAttribute;
    using R = Newtonsoft.Json.Required;
    using N = Newtonsoft.Json.NullValueHandling;

    public partial class FleetCarrierRoutes
    {
        [J("result", NullValueHandling = N.Ignore)] public Result Result { get; set; }
        [J("status", NullValueHandling = N.Ignore)] public string Status { get; set; }
    }

    public partial class Result
    {
        [J("capacity_used", NullValueHandling = N.Ignore)]  public long? CapacityUsed { get; set; } 
        [J("destination", NullValueHandling = N.Ignore)]    public string Destination { get; set; } 
        [J("fuel_loaded", NullValueHandling = N.Ignore)]    public long? FuelLoaded { get; set; }   
        [J("jumps", NullValueHandling = N.Ignore)]          public List<Jump> Jumps { get; set; }   
        [J("source", NullValueHandling = N.Ignore)]         public string Source { get; set; }      
        [J("tritium_stored", NullValueHandling = N.Ignore)] public long? TritiumStored { get; set; }
    }

    public partial class Jump
    {
        [J("distance", NullValueHandling = N.Ignore)]                public double? Distance { get; set; }             
        [J("distance_to_destination", NullValueHandling = N.Ignore)] public double? DistanceToDestination { get; set; }
        [J("fuel_in_tank", NullValueHandling = N.Ignore)]            public long? FuelInTank { get; set; }             
        [J("fuel_used", NullValueHandling = N.Ignore)]               public long? FuelUsed { get; set; }               
        [J("has_icy_ring", NullValueHandling = N.Ignore)]            public bool? HasIcyRing { get; set; }             
        [J("id64", NullValueHandling = N.Ignore)]                    public string Id64 { get; set; }                  
        [J("is_system_pristine", NullValueHandling = N.Ignore)]      public bool? IsSystemPristine { get; set; }       
        [J("must_restock", NullValueHandling = N.Ignore)]            public long? MustRestock { get; set; }            
        [J("name", NullValueHandling = N.Ignore)]                    public string Name { get; set; }                  
        [J("x", NullValueHandling = N.Ignore)]                       public double? X { get; set; }                    
        [J("y", NullValueHandling = N.Ignore)]                       public double? Y { get; set; }                    
        [J("z", NullValueHandling = N.Ignore)]                       public double? Z { get; set; }                    
    }

    public partial class FleetCarrierRoutes
    {
        public static FleetCarrierRoutes FromJson(string json) => JsonConvert.DeserializeObject<FleetCarrierRoutes>(json, CideClient.Structures.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this FleetCarrierRoutes self) => JsonConvert.SerializeObject(self, CideClient.Structures.Converter.Settings);
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
