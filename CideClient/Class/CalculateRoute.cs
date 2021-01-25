using System;
using CideClient.Structures;
using CideClient.Api;
using System.Collections.Generic;
using System.Linq;

namespace CideClient.Class
{
    public class CalculateRoute
    {
        public static void MakeRoute(FleetCarrier FC,string Source, string Destination, string CapacityUsed, string FuelLoaded, string TritiumStored)
        {
            //vrati to skurvenou jsonku kteoru musim naparsovat a ziskat id jobs
            string ResultId = PlotterSpansh.NewRoutes(Source, Destination, CapacityUsed, FuelLoaded, TritiumStored);
            var plotterSpanshResult = CideClient.Structures.PlotterSpansh.PlotterSpanshResult.FromJson(ResultId);
            if (plotterSpanshResult.Status == "queued")
            {
                var ResultJson = PlotterSpansh.Result(plotterSpanshResult.Job);
                if (ResultJson == "CantFindRoute")
                {
                    Console.WriteLine("Nelze najit Route");

                }
                else
                {                
                    var fleetCarrierRoutes = FleetCarrierRoutes.FromJson(ResultJson);

                    if (FC.StatusMode == 2)
                    {
                        // seznam obsahuje i samotnou startovni pozici fc proto -1
                        FC.JumpMax = fleetCarrierRoutes.Result.Jumps.Count - 1; 
                        FC.FuelNeeded = FuelNeededRoute(fleetCarrierRoutes.Result.Jumps, FC);
                    }
                    else if(FC.StatusMode == 3)
                    {
                        FC.JumpRoutes = fleetCarrierRoutes;
                        FC.JumpMax = fleetCarrierRoutes.Result.Jumps.Count - 1; 
                        FC.FuelNeeded = FuelNeededRoute(fleetCarrierRoutes.Result.Jumps, FC);
                        NextSystemFromFCR(fleetCarrierRoutes, FC);
                    }
                }

            }
        }

        public static string NextSystemFromFCR(FleetCarrierRoutes fleetCarrierRoutes, FleetCarrier fC)
        {
            for (int i = 0; i < fleetCarrierRoutes.Result.Jumps.Count; i++)
            {
                if (fleetCarrierRoutes.Result.Jumps[i].Name == fC.Location)
                {
                    if (fleetCarrierRoutes.Result.Jumps[i] != fleetCarrierRoutes.Result.Jumps.Last()) // dat if na max počet věci v listu
                    {
                        fC.NextJump = fleetCarrierRoutes.Result.Jumps[i+1].Name;
                        fC.NextJumpEM = fleetCarrierRoutes.Result.Jumps[i+1].Name;
                        return fleetCarrierRoutes.Result.Jumps[i+1].Name;
                    }
                }
            }
            return null;
        }

 
        public static long? FuelNeededRoute(List<Jump> jumps, FleetCarrier fC)
        {
            long? fuelneed = 0;
            bool where = false;
            foreach (var jump in jumps)
            {
                if (where)
                {
                    fuelneed = fuelneed + jump.FuelUsed;
                }
                
                if (jump.Name == fC.Location)
                {
                    where = true;
                }
            }
            return fuelneed;
        }

        public static long? FuelNeededRouteOneJump(List<Jump> jumps, FleetCarrier fleetCarrier)
        {
            bool where = false;
            foreach (var jump in jumps)
            {
                if (where)
                {
                    return jump.FuelUsed;
                }
                
                if (jump.Name == fleetCarrier.Location)
                {
                    where = true;
                }
            }
            return null;
        }

        internal static long? CalculateJumpLeft(FleetCarrier fleetCarrier)
        {
            long? JumpToFinish = 0;
            bool where = false;
            foreach (var jump in fleetCarrier.JumpRoutes.Result.Jumps)
            {
                if (where)
                {
                    JumpToFinish = JumpToFinish + 1;
                }
                
                if (jump.Name == fleetCarrier.Location)
                {
                    where = true;
                }
            }
            return JumpToFinish;
        }
    }
}